namespace VehicleTracking.Infrastructure.Data.MongoDb
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string VehiclesCollectionName { get; set; } = "Vehicles";
        public string LocationHistoryCollectionName { get; set; } = "LocationHistory";
        public string UsersCollectionName { get; set; } = "Users";
        public string RolesCollectionName { get; set; } = "Roles";
        public string PermissionsCollectionName { get; set; } = "Permissions";
        public string TenantsCollectionName { get; set; } = "Tenants";
        public string DevicesCollectionName { get; set; } = "Devices";
        public string TenantCustomFieldsCollectionName { get; set; } = "TenantCustomFields";
        public string UserRolesCollectionName { get; set; } = "UserRoles";
        public string RolePermissionsCollectionName { get; set; } = "RolePermissions";
    }
} 