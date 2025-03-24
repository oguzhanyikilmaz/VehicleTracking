namespace VehicleTracking.Infrastructure.Data.MongoDb
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string VehiclesCollectionName { get; set; } = string.Empty;
        public string LocationHistoryCollectionName { get; set; } = string.Empty;
    }
} 