using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace VehicleTracking.Infrastructure.TCP.Services
{
    public class TcpConnectionPool : IDisposable
    {
        private readonly ILogger<TcpConnectionPool> _logger;
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentDictionary<string, ConcurrentQueue<TcpClient>> _pool;
        private readonly CancellationTokenSource _cleanupTokenSource;
        private readonly Task _cleanupTask;
        private readonly int _maxConnectionsPerKey;
        private readonly TimeSpan _connectionTimeout;

        public TcpConnectionPool(
            ILogger<TcpConnectionPool> logger, 
            int maxConnections = 100,
            int maxConnectionsPerKey = 10,
            int connectionTimeoutMs = 60000)
        {
            _logger = logger;
            _semaphore = new SemaphoreSlim(maxConnections);
            _pool = new ConcurrentDictionary<string, ConcurrentQueue<TcpClient>>();
            _maxConnectionsPerKey = maxConnectionsPerKey;
            _connectionTimeout = TimeSpan.FromMilliseconds(connectionTimeoutMs);
            _cleanupTokenSource = new CancellationTokenSource();
            _cleanupTask = Task.Run(() => CleanupAsync(_cleanupTokenSource.Token));
        }

        public async Task<TcpClient> GetConnectionAsync(string key, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                if (!_pool.TryGetValue(key, out var queue))
                {
                    queue = new ConcurrentQueue<TcpClient>();
                    _pool.TryAdd(key, queue);
                }

                // Try to get a connection from the pool
                if (queue.TryDequeue(out var client))
                {
                    if (IsConnectionValid(client))
                    {
                        _logger.LogDebug("Reusing connection from pool for key: {Key}", key);
                        return client;
                    }
                    else
                    {
                        // Connection is no longer valid, dispose it
                        try { client.Dispose(); } catch { }
                    }
                }

                // Create a new connection
                _logger.LogDebug("Creating new connection for key: {Key}", key);
                return new TcpClient();
            }
            catch (Exception ex)
            {
                _semaphore.Release();
                _logger.LogError(ex, "Error getting connection from pool for key: {Key}", key);
                throw;
            }
        }

        public void ReturnConnection(string key, TcpClient client)
        {
            if (client == null || !IsConnectionValid(client))
            {
                try { client?.Dispose(); } 
                catch { }
                _semaphore.Release();
                return;
            }

            if (!_pool.TryGetValue(key, out var queue))
            {
                queue = new ConcurrentQueue<TcpClient>();
                _pool.TryAdd(key, queue);
            }

            // Check if we should add the connection back to the pool
            int queuedCount = queue.Count;
            if (queuedCount < _maxConnectionsPerKey)
            {
                queue.Enqueue(client);
                _logger.LogDebug("Returned connection to pool for key: {Key}, pool size: {Count}", key, queuedCount + 1);
            }
            else
            {
                // Pool is full, dispose the connection
                try { client.Dispose(); } 
                catch { }
                _logger.LogDebug("Pool full for key: {Key}, disposing connection", key);
            }

            _semaphore.Release();
        }

        private bool IsConnectionValid(TcpClient client)
        {
            if (client == null)
                return false;

            try
            {
                return client.Connected && client.Client != null && client.Client.Connected;
            }
            catch
            {
                return false;
            }
        }

        private async Task CleanupAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Wait before cleanup (every 30 seconds)
                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);

                    foreach (var key in _pool.Keys)
                    {
                        if (_pool.TryGetValue(key, out var queue))
                        {
                            int count = queue.Count;
                            if (count > 0)
                            {
                                _logger.LogDebug("Cleaning up idle connections for key: {Key}, count: {Count}", key, count);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected when token is canceled
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during connection pool cleanup");
                }
            }
        }

        public void Dispose()
        {
            _cleanupTokenSource.Cancel();
            
            try
            {
                _cleanupTask.Wait(TimeSpan.FromSeconds(5));
            }
            catch { }

            foreach (var queue in _pool.Values)
            {
                while (queue.TryDequeue(out var client))
                {
                    try { client.Dispose(); } 
                    catch { }
                }
            }

            _pool.Clear();
            _semaphore.Dispose();
            _cleanupTokenSource.Dispose();
        }
    }
} 