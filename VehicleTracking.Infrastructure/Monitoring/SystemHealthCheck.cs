using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace VehicleTracking.Infrastructure.Monitoring
{
    public class SystemHealthCheck : IHealthCheck
    {
        private readonly ILogger<SystemHealthCheck> _logger;
        private readonly PerformanceMetrics _performanceMetrics;
        private DateTime _lastHealthCheckTime = DateTime.UtcNow;
        private bool _healthyLastCheck = true;

        public SystemHealthCheck(ILogger<SystemHealthCheck> logger, PerformanceMetrics performanceMetrics)
        {
            _logger = logger;
            _performanceMetrics = performanceMetrics;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (_performanceMetrics.MeasureOperation("HealthCheck"))
                {
                    var cpuUsage = GetCpuUsage();
                    var memoryUsage = GetMemoryUsage();
                    var threadCount = GetThreadCount();
                    
                    var data = new Dictionary<string, object>
                    {
                        { "CpuUsage", $"{cpuUsage:F2}%" },
                        { "MemoryUsageMB", $"{memoryUsage:F0} MB" },
                        { "ThreadCount", threadCount },
                        { "LastCheckTime", _lastHealthCheckTime },
                        { "UpTime", GetUpTime() }
                    };

                    _lastHealthCheckTime = DateTime.UtcNow;

                    var healthy = cpuUsage < 90 && memoryUsage < 2048; // 90% CPU ve 2GB RAM limiti
                    
                    if (!healthy && _healthyLastCheck)
                    {
                        _logger.LogWarning("System health degraded: CPU={CpuUsage}%, Memory={MemoryUsageMB}MB", 
                            cpuUsage, memoryUsage);
                    }
                    else if (healthy && !_healthyLastCheck)
                    {
                        _logger.LogInformation("System health recovered: CPU={CpuUsage}%, Memory={MemoryUsageMB}MB", 
                            cpuUsage, memoryUsage);
                    }

                    _healthyLastCheck = healthy;

                    if (healthy)
                    {
                        return HealthCheckResult.Healthy("System running normally", data);
                    }
                    else
                    {
                        return HealthCheckResult.Degraded("System resources under high load", null, data);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during health check");
                return HealthCheckResult.Unhealthy("Error during health check", ex);
            }
        }

        private double GetCpuUsage()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            
            Thread.Sleep(100); // 100ms ölçüm için bekle
            
            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            
            var cpuUsagePercent = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed) * 100;
            
            return Math.Round(cpuUsagePercent, 2);
        }

        private double GetMemoryUsage()
        {
            var process = Process.GetCurrentProcess();
            return process.WorkingSet64 / 1024.0 / 1024.0; // MB cinsinden
        }

        private int GetThreadCount()
        {
            var process = Process.GetCurrentProcess();
            return process.Threads.Count;
        }

        private TimeSpan GetUpTime()
        {
            var process = Process.GetCurrentProcess();
            return DateTime.Now - process.StartTime;
        }
    }
} 