using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Domain.Repositories;
using VehicleTracking.Infrastructure.Data;

namespace VehicleTracking.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserRepository(MongoDbContext context)
        {
            _usersCollection = context.Users;
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _usersCollection.Find(_ => true).ToListAsync();
        }

        public async Task<List<User>> GetByTenantIdAsync(string tenantId)
        {
            return await _usersCollection.Find(u => u.TenantId == tenantId).ToListAsync();
        }

        public async Task<User> AddAsync(User user)
        {
            await _usersCollection.InsertOneAsync(user);
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _usersCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
            return user;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _usersCollection.DeleteOneAsync(u => u.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> ExistsByIdAsync(string id)
        {
            return await _usersCollection.Find(u => u.Id == id).AnyAsync();
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _usersCollection.Find(u => u.Username == username).AnyAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _usersCollection.Find(u => u.Email == email).AnyAsync();
        }

        public async Task<bool> UpdateRefreshTokenAsync(string id, string refreshToken, DateTime refreshTokenExpiryTime)
        {
            var update = Builders<User>.Update
                .Set(u => u.RefreshToken, refreshToken)
                .Set(u => u.RefreshTokenExpiryTime, refreshTokenExpiryTime)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            var result = await _usersCollection.UpdateOneAsync(u => u.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateLastLoginTimeAsync(string id)
        {
            var update = Builders<User>.Update
                .Set(u => u.LastLoginAt, DateTime.UtcNow)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            var result = await _usersCollection.UpdateOneAsync(u => u.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<User> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _usersCollection.Find(u => u.RefreshToken == refreshToken).FirstOrDefaultAsync();
        }

        public async Task<bool> RevokeRefreshTokenAsync(string id)
        {
            var update = Builders<User>.Update
                .Set(u => u.RefreshToken, string.Empty)
                .Set(u => u.RefreshTokenExpiryTime, null)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            var result = await _usersCollection.UpdateOneAsync(u => u.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> ConfirmEmailAsync(string id)
        {
            var update = Builders<User>.Update
                .Set(u => u.EmailConfirmed, true)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            var result = await _usersCollection.UpdateOneAsync(u => u.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> AddToRoleAsync(string userId, string roleId)
        {
            var update = Builders<User>.Update
                .AddToSet(u => u.RoleIds, roleId)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            var result = await _usersCollection.UpdateOneAsync(u => u.Id == userId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> RemoveFromRoleAsync(string userId, string roleId)
        {
            var update = Builders<User>.Update
                .Pull(u => u.RoleIds, roleId)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            var result = await _usersCollection.UpdateOneAsync(u => u.Id == userId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<List<string>> GetUserRoleIdsAsync(string userId)
        {
            var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            return user?.RoleIds ?? new List<string>();
        }

        public async Task<bool> UpdatePasswordAsync(string id, string passwordHash)
        {
            var update = Builders<User>.Update
                .Set(u => u.PasswordHash, passwordHash)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            var result = await _usersCollection.UpdateOneAsync(u => u.Id == id, update);
            return result.ModifiedCount > 0;
        }
    }
} 