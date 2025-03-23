using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace VehicleTracking.Infrastructure.Monitoring
{
    public class PerformanceMetrics
    {
        private readonly ILogger<PerformanceMetrics> _logger;
        private readonly ConcurrentDictionary<string, MetricData> _metrics = new ConcurrentDictionary<string, MetricData>();
        private readonly Timer _reportingTimer;
        private readonly TimeSpan _reportingInterval;

        public PerformanceMetrics(ILogger<PerformanceMetrics> logger, TimeSpan? reportingInterval = null)
        {
            _logger = logger;
            _reportingInterval = reportingInterval ?? TimeSpan.FromMinutes(5);
            _reportingTimer = new Timer(ReportMetrics, null, _reportingInterval, _reportingInterval);
        }

        public IDisposable MeasureOperation(string operationName)
        {
            return new OperationMeasurement(this, operationName);
        }

        public void TrackOperationTime(string operationName, long elapsedMilliseconds)
        {
            var metricData = _metrics.GetOrAdd(operationName, _ => new MetricData());
            
            metricData.Count++;
            metricData.TotalTime += elapsedMilliseconds;
            
            if (elapsedMilliseconds > metricData.MaxTime)
            {
                metricData.MaxTime = elapsedMilliseconds;
            }
            
            if (elapsedMilliseconds < metricData.MinTime || metricData.MinTime == 0)
            {
                metricData.MinTime = elapsedMilliseconds;
            }
        }

        public void IncrementCounter(string counterName, int incrementBy = 1)
        {
            var metricData = _metrics.GetOrAdd(counterName, _ => new MetricData());
            Interlocked.Add(ref metricData.Count, incrementBy);
        }

        private void ReportMetrics(object state)
        {
            try
            {
                _logger.LogInformation("Performance Metrics Report:");
                foreach (var metric in _metrics)
                {
                    var data = metric.Value;
                    
                    if (data.Count > 0)
                    {
                        var avgTime = data.TotalTime / data.Count;
                        _logger.LogInformation(
                            "{MetricName}: Count={Count}, Avg={AvgTime}ms, Min={MinTime}ms, Max={MaxTime}ms",
                            metric.Key, data.Count, avgTime, data.MinTime, data.MaxTime);
                    }
                    else
                    {
                        _logger.LogInformation("{MetricName}: Count={Count}", metric.Key, data.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reporting performance metrics");
            }
        }

        private class MetricData
        {
            public long Count;
            public long TotalTime;
            public long MinTime;
            public long MaxTime;
        }

        private class OperationMeasurement : IDisposable
        {
            private readonly PerformanceMetrics _metrics;
            private readonly string _operationName;
            private readonly Stopwatch _stopwatch;
            private bool _disposed;

            public OperationMeasurement(PerformanceMetrics metrics, string operationName)
            {
                _metrics = metrics;
                _operationName = operationName;
                _stopwatch = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                if (_disposed) return;
                
                _stopwatch.Stop();
                _metrics.TrackOperationTime(_operationName, _stopwatch.ElapsedMilliseconds);
                _disposed = true;
            }
        }
    }
} 