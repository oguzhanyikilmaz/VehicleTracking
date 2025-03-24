using Microsoft.EntityFrameworkCore;
using VehicleTracking.API.Hubs;
using VehicleTracking.API.Services;
using VehicleTracking.Application.Mapping;
using VehicleTracking.Application.Services;
using VehicleTracking.Domain.Repositories;
using VehicleTracking.Infrastructure.Common;
using VehicleTracking.Infrastructure.Data;
using VehicleTracking.Infrastructure.Data.BulkUpdates;
using VehicleTracking.Infrastructure.Monitoring;
using VehicleTracking.Infrastructure.Repositories;
using VehicleTracking.Infrastructure.TCP.Interfaces;
using VehicleTracking.Infrastructure.TCP.Models;
using VehicleTracking.Infrastructure.TCP.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

// Static files for web UI
builder.Services.AddDirectoryBrowser();

// SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 102400; // 100 KB
});

// Database
builder.Services.AddDbContext<VehicleTrackingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), 
        sqlOptions => sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<VehicleTrackingDbContext>("Database")
    .AddCheck<SystemHealthCheck>("System");

// Services
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

// Broadcast Service
builder.Services.AddScoped<ILocationBroadcastService, LocationBroadcastService>();

// Common Services
builder.Services.AddSingleton<RetryPolicy>();
builder.Services.AddSingleton<PerformanceMetrics>();

// Bulk Update Services
builder.Services.AddScoped<BulkUpdateService>();

// TCP Services
builder.Services.AddSingleton<ITcpDataProcessor, TcpDataProcessor>();
builder.Services.AddSingleton(provider => {
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
    return loggerFactory.CreateLogger<BatchProcessor<TcpLocationData>>();
});
builder.Services.AddSingleton(provider => {
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
    return loggerFactory.CreateLogger<TcpConnectionPool>();
});

// Configure dependency injection for hosted services that need scoped services
builder.Services.AddScoped<TcpServerFactory>();
builder.Services.AddHostedService<TcpServerHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Health check endpoint
app.MapHealthChecks("/health");

// SignalR hub endpoints
app.MapHub<VehicleLocationHub>("/hubs/vehicleLocation");

app.MapControllers();

// WWWRoot dizini oluşturma
var webRootPath = app.Environment.WebRootPath;
if (string.IsNullOrEmpty(webRootPath)) 
{
    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    if (!Directory.Exists(webRootPath))
    {
        Directory.CreateDirectory(webRootPath);
    }
}

app.Run();

// Hosted service için factory class, scoped servisleri kullanabilmek için
public class TcpServerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public TcpServerFactory(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public TcpServer CreateTcpServer()
    {
        var logger = _serviceProvider.GetRequiredService<ILogger<TcpServer>>();
        var dataProcessor = _serviceProvider.GetRequiredService<ITcpDataProcessor>();
        var vehicleRepository = _serviceProvider.GetRequiredService<IVehicleRepository>();
        var bulkUpdateService = _serviceProvider.GetRequiredService<BulkUpdateService>();
        var retryPolicy = _serviceProvider.GetRequiredService<RetryPolicy>();
        var performanceMetrics = _serviceProvider.GetRequiredService<PerformanceMetrics>();
        var vehicleService = _serviceProvider.GetRequiredService<IVehicleService>();
        var locationBroadcastService = _serviceProvider.GetRequiredService<ILocationBroadcastService>();
        var batchProcessorLogger = _serviceProvider.GetRequiredService<ILogger<BatchProcessor<TcpLocationData>>>();
        var connectionPoolLogger = _serviceProvider.GetRequiredService<ILogger<TcpConnectionPool>>();

        var port = _configuration.GetSection("TcpServer").GetValue<int>("Port", 5000);
        
        return new TcpServer(
            logger,
            dataProcessor,
            vehicleRepository,
            bulkUpdateService,
            retryPolicy,
            performanceMetrics,
            vehicleService,
            locationBroadcastService,
            batchProcessorLogger,
            connectionPoolLogger,
            port);
    }
}

public class TcpServerHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private TcpServer _tcpServer;

    public TcpServerHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var factory = scope.ServiceProvider.GetRequiredService<TcpServerFactory>();
            _tcpServer = factory.CreateTcpServer();
            await _tcpServer.StartAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_tcpServer != null)
        {
            await _tcpServer.StopAsync(cancellationToken);
            (_tcpServer as IDisposable)?.Dispose();
        }
    }
} 