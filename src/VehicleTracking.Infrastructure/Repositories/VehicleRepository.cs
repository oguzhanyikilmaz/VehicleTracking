using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Domain.Repositories;
using VehicleTracking.Infrastructure.Data.MongoDb;

namespace VehicleTracking.Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly MongoDbContext _context;

        public VehicleRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<Vehicle> GetByIdAsync(string id)
        {
            return await _context.Vehicles.Find(v => v.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles.Find(_ => true).ToListAsync();
        }

        public async Task<Vehicle> AddAsync(Vehicle vehicle)
        {
            await _context.Vehicles.InsertOneAsync(vehicle);
            return vehicle;
        }

        public async Task<Vehicle> UpdateAsync(Vehicle vehicle)
        {
            var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, vehicle.Id);
            var result = await _context.Vehicles.ReplaceOneAsync(filter, vehicle);
            return result.ModifiedCount > 0 ? vehicle : null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, id);
            var result = await _context.Vehicles.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<bool> UpdateLocationAsync(string id, GeoJsonPoint<GeoJson2DGeographicCoordinates> location, double speed)
        {
            var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, id);
            var update = Builders<Vehicle>.Update
                .Set(v => v.Location, location)
                .Set(v => v.Speed, speed)
                .Set(v => v.LastUpdateTime, DateTime.UtcNow);

            var result = await _context.Vehicles.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<IEnumerable<Vehicle>> GetActiveVehiclesAsync()
        {
            return await _context.Vehicles.Find(v => v.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByIdsAsync(IEnumerable<string> ids)
        {
            var filter = Builders<Vehicle>.Filter.In(v => v.Id, ids);
            return await _context.Vehicles.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesNearLocationAsync(double latitude, double longitude, double maxDistanceKm)
        {
            // MongoDB metreyle çalışır, km'yi metreye çeviriyoruz
            var maxDistanceMeters = maxDistanceKm * 1000;

            // MongoDB için json sorgusu - aktif araçları belirli bir mesafede arar
            var filterJson = $@"{{
                '$and': [
                    {{
                        'Location': {{
                            '$nearSphere': {{
                                '$geometry': {{
                                    'type': 'Point',
                                    'coordinates': [{longitude}, {latitude}]
                                }},
                                '$maxDistance': {maxDistanceMeters}
                            }}
                        }}
                    }},
                    {{
                        'IsActive': true
                    }}
                ]
            }}";

            // JSON stringini MongoDB filtresine dönüştür
            var filter = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(filterJson);
            
            // Filtreyi uygula
            return await _context.Vehicles.Find(filter).ToListAsync();
        }

        public async Task<Vehicle> GetVehicleByDeviceIdAsync(string deviceId)
        {
            return await _context.Vehicles.Find(v => v.DeviceId == deviceId).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateDeviceIdAsync(string vehicleId, string deviceId)
        {
            var update = Builders<Vehicle>.Update
                .Set(v => v.DeviceId, deviceId)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _context.Vehicles.UpdateOneAsync(v => v.Id == vehicleId, update);
            return result.ModifiedCount > 0;
        }
    }
} 