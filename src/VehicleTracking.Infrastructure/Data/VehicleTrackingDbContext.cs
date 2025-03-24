using VehicleTracking.Domain.Entities;

namespace VehicleTracking.Infrastructure.Data
{
    public class VehicleTrackingDbContext //: DbContext
    {
        //public VehicleTrackingDbContext(DbContextOptions<VehicleTrackingDbContext> options)
        //    : base(options)
        //{
        //}

        //public DbSet<Vehicle> Vehicles { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Vehicle>(entity =>
        //    {
        //        entity.HasKey(e => e.Id);
        //        entity.Property(e => e.PlateNumber).IsRequired();
        //        entity.Property(e => e.DeviceId).IsRequired();
        //        entity.Property(e => e.LastUpdateTime).HasDefaultValueSql("GETDATE()");
        //    });
        //}
    }
}