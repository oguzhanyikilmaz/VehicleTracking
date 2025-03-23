using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace VehicleTracking.Infrastructure.Common
{
    public class BatchProcessor<T>
    {
        private readonly ILogger<BatchProcessor<T>> _logger;
        private readonly BlockingCollection<T> _queue;
        private readonly Func<IEnumerable<T>, Task> _processBatchAsync;
        private readonly int _maxBatchSize;
        private readonly TimeSpan _maxBatchDelay;
        private readonly Task _processingTask;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public BatchProcessor(
            ILogger<BatchProcessor<T>> logger,
            Func<IEnumerable<T>, Task> processBatchAsync,
            int maxBatchSize = 100,
            int maxBatchDelayMs = 1000)
        {
            _logger = logger;
            _processBatchAsync = processBatchAsync;
            _maxBatchSize = maxBatchSize;
            _maxBatchDelay = TimeSpan.FromMilliseconds(maxBatchDelayMs);
            _queue = new BlockingCollection<T>(new ConcurrentQueue<T>());
            _cancellationTokenSource = new CancellationTokenSource();
            _processingTask = Task.Run(() => ProcessQueueAsync(_cancellationTokenSource.Token));
        }

        public void Add(T item)
        {
            if (_cancellationTokenSource.IsCancellationRequested)
                throw new InvalidOperationException("Batch processor is stopped");

            _queue.Add(item);
        }

        public async Task StopAsync()
        {
            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            _cancellationTokenSource.Cancel();
            _queue.CompleteAdding();

            try
            {
                await _processingTask;
            }
            catch (OperationCanceledException)
            {
                // Expected cancellation, ignore
            }
        }

        private async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var batch = new List<T>(_maxBatchSize);
                    var batchStartTime = DateTime.UtcNow;
                    var timeoutToken = new CancellationTokenSource(_maxBatchDelay).Token;
                    var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutToken).Token;

                    // Try to fill batch until max size or timeout
                    try
                    {
                        while (batch.Count < _maxBatchSize && !combinedToken.IsCancellationRequested)
                        {
                            if (_queue.TryTake(out var item, 100, cancellationToken))
                            {
                                batch.Add(item);
                            }
                            else if (batch.Count > 0)
                            {
                                // If we have items but can't get more, process what we have
                                break;
                            }
                        }
                    }
                    catch (OperationCanceledException) when (timeoutToken.IsCancellationRequested)
                    {
                        // This is the timeout, which is expected
                    }

                    // Process batch if we have any items
                    if (batch.Count > 0)
                    {
                        try
                        {
                            _logger.LogDebug("Processing batch of {Count} items", batch.Count);
                            await _processBatchAsync(batch);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing batch of {Count} items", batch.Count);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when token is canceled
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in batch processing loop");
            }
        }
    }
} 