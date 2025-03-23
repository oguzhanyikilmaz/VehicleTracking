using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace VehicleTracking.Infrastructure.Common
{
    public class RetryPolicy
    {
        private readonly ILogger<RetryPolicy> _logger;

        public RetryPolicy(ILogger<RetryPolicy> logger)
        {
            _logger = logger;
        }

        public AsyncRetryPolicy GetDefaultRetryPolicy()
        {
            return Policy
                .Handle<Exception>(ex => !(ex is OperationCanceledException))
                .WaitAndRetryAsync(
                    3, // retry count
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // exponential backoff
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, "Retry {RetryCount} of {Context} after {Delay}ms", 
                            retryCount, context["OperationKey"], timeSpan.TotalMilliseconds);
                    }
                );
        }

        public AsyncRetryPolicy GetDatabaseRetryPolicy()
        {
            return Policy
                .Handle<Exception>(ex => !(ex is OperationCanceledException)) 
                .WaitAndRetryAsync(
                    5, // more retries for DB
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(1.5, retryAttempt)), // less aggressive backoff
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, "Database retry {RetryCount} of {Context} after {Delay}ms", 
                            retryCount, context["OperationKey"], timeSpan.TotalMilliseconds);
                    }
                );
        }

        public AsyncRetryPolicy GetTcpRetryPolicy()
        {
            return Policy
                .Handle<SocketException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(retryAttempt),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, "TCP retry {RetryCount} of {Context} after {Delay}ms", 
                            retryCount, context["OperationKey"], timeSpan.TotalMilliseconds);
                    }
                );
        }
    }
} 