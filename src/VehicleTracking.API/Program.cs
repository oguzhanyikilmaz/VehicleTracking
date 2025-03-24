using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using VehicleTracking.API.Hubs;
using VehicleTracking.API.Services;
using VehicleTracking.Application.Mapping;
using VehicleTracking.Application.Services;
using VehicleTracking.Domain.Repositories;
using VehicleTracking.Infrastructure.Common;
using VehicleTracking.Infrastructure.Data.BulkUpdates;
using VehicleTracking.Infrastructure.Data.MongoDb;
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

// MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Health checks
var healthCheckBuilder = builder.Services.AddHealthChecks();
healthCheckBuilder.AddCheck<SystemHealthCheck>("System");

// Health Checks için MongoDB eklentisi
var mongoConnectionString = builder.Configuration.GetSection("MongoDbSettings:ConnectionString").Value;
if (!string.IsNullOrEmpty(mongoConnectionString))
{
    // MongoDB sağlık kontrolü basit bir şekilde ekleniyor
    healthCheckBuilder.Add(
        new HealthCheckRegistration(
            "MongoDB",
            sp => new VehicleTracking.Infrastructure.Monitoring.MongoDbHealthCheck(mongoConnectionString), 
            HealthStatus.Degraded,
            new[] { "mongodb", "database" }));
}

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
builder.Services.AddSingleton(provider => 
    provider.GetRequiredService<ILoggerFactory>().CreateLogger<BatchProcessor<TcpLocationData>>());
builder.Services.AddSingleton(provider => 
    provider.GetRequiredService<ILoggerFactory>().CreateLogger<TcpConnectionPool>());
builder.Services.AddSingleton(provider => 
    provider.GetRequiredService<ILoggerFactory>().CreateLogger<TcpServer>());

// Configure dependency injection for hosted services that need scoped services
builder.Services.AddScoped<TcpServerFactory>();
builder.Services.AddHostedService<TcpServerHostedService>();

var app = builder.Build();

// MongoDB bağlantısını kontrol et
using (var scope = app.Services.CreateScope())
{
    var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try 
    {
        // MongoDB bağlantısı ve indeksleri oluştur
        logger.LogInformation("MongoDB bağlantısı kontrol ediliyor...");
        // MongoDbContext constructor'ında otomatik olarak indeksler oluşturuluyor
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "MongoDB bağlantısı kurulamadı!");
        // Hata olduğunda uygulamayı durdurma - geliştirme ortamında bunu açabilirsiniz
        // throw;
    }
}

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

// ILogger<Program> için gerekli olan Program sınıfı tanımı
public partial class Program { }

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
        var scope = _serviceProvider.CreateScope();
        
        var logger = _serviceProvider.GetRequiredService<ILogger<TcpServer>>();
        var dataProcessor = _serviceProvider.GetRequiredService<ITcpDataProcessor>();
        var vehicleRepository = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();
        var bulkUpdateService = scope.ServiceProvider.GetRequiredService<BulkUpdateService>();
        var retryPolicy = _serviceProvider.GetRequiredService<RetryPolicy>();
        var performanceMetrics = _serviceProvider.GetRequiredService<PerformanceMetrics>();
        var vehicleService = scope.ServiceProvider.GetRequiredService<IVehicleService>();
        var locationBroadcastService = scope.ServiceProvider.GetRequiredService<ILocationBroadcastService>();
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

public class TcpServerHostedService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private TcpServer? _tcpServer;
    private IServiceScope? _scope;

    public TcpServerHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Scoped servislerin kullanım ömrünü TCP sunucusu ile eşleştirmek için scope oluştur
        _scope = _serviceProvider.CreateScope();
        
        try
        {
            var factory = _scope.ServiceProvider.GetRequiredService<TcpServerFactory>();
            _tcpServer = factory.CreateTcpServer();
            
            if (_tcpServer != null)
            {
                await _tcpServer.StartAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            var logger = _scope.ServiceProvider.GetRequiredService<ILogger<TcpServerHostedService>>();
            logger.LogError(ex, "TCP sunucusu başlatılırken hata oluştu");
            
            // Hata durumunda kaynakları temizle
            _scope.Dispose();
            _scope = null;
            
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_tcpServer != null)
        {
            try
            {
                await _tcpServer.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                if (_scope != null)
                {
                    var logger = _scope.ServiceProvider.GetRequiredService<ILogger<TcpServerHostedService>>();
                    logger.LogError(ex, "TCP sunucusu durdurulurken hata oluştu");
                }
            }
        }
    }
    
    public void Dispose()
    {
        (_tcpServer as IDisposable)?.Dispose();
        _scope?.Dispose();
        GC.SuppressFinalize(this);
    }
} 