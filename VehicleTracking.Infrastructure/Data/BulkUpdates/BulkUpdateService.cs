using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using VehicleTracking.Infrastructure.Common;

namespace VehicleTracking.Infrastructure.Data.BulkUpdates
{
    public class BulkUpdateService
    {
        private readonly VehicleTrackingDbContext _dbContext;
        private readonly ILogger<BulkUpdateService> _logger;
        private readonly RetryPolicy _retryPolicy;

        public BulkUpdateService(
            VehicleTrackingDbContext dbContext,
            ILogger<BulkUpdateService> logger,
            RetryPolicy retryPolicy)
        {
            _dbContext = dbContext;
            _logger = logger;
            _retryPolicy = retryPolicy;
        }

        public async Task BulkUpdateLocationsAsync(IEnumerable<BulkLocationUpdate> updates)
        {
            if (!updates.Any())
                return;

            try
            {
                await _retryPolicy.GetDatabaseRetryPolicy().ExecuteAsync(async context =>
                {
                    var connectionString = _dbContext.Database.GetConnectionString();
                    using var connection = new SqlConnection(connectionString);
                    await connection.OpenAsync();

                    using var transaction = connection.BeginTransaction();
                    try
                    {
                        using var dataTable = CreateDataTable(updates);
                        await BulkCopyAsync(connection, transaction, dataTable);
                        
                        // Execute the merge stored procedure
                        using var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        command.CommandText = "EXEC dbo.MergeVehicleLocations";
                        command.CommandType = CommandType.Text;
                        await command.ExecuteNonQueryAsync();
                        
                        transaction.Commit();
                        _logger.LogInformation("Bulk updated {Count} vehicle locations", updates.Count());
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }, new Context { ["OperationKey"] = "BulkUpdateLocations" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk location update for {Count} vehicles", updates.Count());
                throw;
            }
        }

        private DataTable CreateDataTable(IEnumerable<BulkLocationUpdate> updates)
        {
            var dataTable = new DataTable("VehicleLocationUpdates");
            
            dataTable.Columns.Add("VehicleId", typeof(Guid));
            dataTable.Columns.Add("Latitude", typeof(double));
            dataTable.Columns.Add("Longitude", typeof(double));
            dataTable.Columns.Add("Speed", typeof(double));
            dataTable.Columns.Add("UpdatedAt", typeof(DateTime));

            foreach (var update in updates)
            {
                dataTable.Rows.Add(
                    update.VehicleId,
                    update.Latitude,
                    update.Longitude,
                    update.Speed,
                    update.Timestamp
                );
            }

            return dataTable;
        }

        private async Task BulkCopyAsync(SqlConnection connection, SqlTransaction transaction, DataTable dataTable)
        {
            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction)
            {
                DestinationTableName = "VehicleLocationUpdates",
                BatchSize = 1000,
                BulkCopyTimeout = 60 // seconds
            };

            bulkCopy.ColumnMappings.Add("VehicleId", "VehicleId");
            bulkCopy.ColumnMappings.Add("Latitude", "Latitude");
            bulkCopy.ColumnMappings.Add("Longitude", "Longitude");
            bulkCopy.ColumnMappings.Add("Speed", "Speed");
            bulkCopy.ColumnMappings.Add("UpdatedAt", "UpdatedAt");

            await bulkCopy.WriteToServerAsync(dataTable);
        }

        // SQL kodunu oluşturan yardımcı metot
        public string GenerateMergeStoredProcedure()
        {
            var sql = new StringBuilder();
            sql.AppendLine("CREATE OR ALTER PROCEDURE dbo.MergeVehicleLocations");
            sql.AppendLine("AS");
            sql.AppendLine("BEGIN");
            sql.AppendLine("    SET NOCOUNT ON;");
            sql.AppendLine();
            sql.AppendLine("    MERGE INTO Vehicles AS target");
            sql.AppendLine("    USING (");
            sql.AppendLine("        SELECT");
            sql.AppendLine("            VehicleId,");
            sql.AppendLine("            Latitude,");
            sql.AppendLine("            Longitude,");
            sql.AppendLine("            Speed,");
            sql.AppendLine("            UpdatedAt");
            sql.AppendLine("        FROM VehicleLocationUpdates");
            sql.AppendLine("    ) AS source ON (target.Id = source.VehicleId)");
            sql.AppendLine("    WHEN MATCHED THEN");
            sql.AppendLine("        UPDATE SET");
            sql.AppendLine("            target.Latitude = source.Latitude,");
            sql.AppendLine("            target.Longitude = source.Longitude,");
            sql.AppendLine("            target.Speed = source.Speed,");
            sql.AppendLine("            target.LastUpdateTime = source.UpdatedAt;");
            sql.AppendLine();
            sql.AppendLine("    -- Temporary tabloyu temizle");
            sql.AppendLine("    TRUNCATE TABLE VehicleLocationUpdates;");
            sql.AppendLine("END");

            return sql.ToString();
        }

        // Geçici tablo oluşturan SQL
        public string CreateTemporaryTable()
        {
            return @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VehicleLocationUpdates')
                BEGIN
                    CREATE TABLE VehicleLocationUpdates (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        VehicleId UNIQUEIDENTIFIER NOT NULL,
                        Latitude FLOAT NOT NULL,
                        Longitude FLOAT NOT NULL, 
                        Speed FLOAT NOT NULL,
                        UpdatedAt DATETIME NOT NULL
                    );
                    
                    CREATE INDEX IX_VehicleLocationUpdates_VehicleId ON VehicleLocationUpdates(VehicleId);
                END
            ";
        }

        // Veritabanı başlangıcında gerekli nesneleri oluşturmak için
        public async Task EnsureDatabaseObjectsAsync()
        {
            try
            {
                // Geçici tabloyu oluştur
                await _dbContext.Database.ExecuteSqlRawAsync(CreateTemporaryTable());
                
                // Stored procedure'ü oluştur veya güncelle
                await _dbContext.Database.ExecuteSqlRawAsync(GenerateMergeStoredProcedure());
                
                _logger.LogInformation("Database objects for bulk operations created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating database objects for bulk updates");
                throw;
            }
        }
    }
} 