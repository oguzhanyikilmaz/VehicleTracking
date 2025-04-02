using System;

namespace VehicleTracking.Application.DTOs
{
    public class TenantCustomFieldDto
    {
        public string Id { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;
        public string FieldValue { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
    
    public class TenantCustomFieldCreateDto
    {
        public string TenantId { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;
        public string FieldValue { get; set; } = string.Empty;
    }
    
    public class TenantCustomFieldUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;
        public string FieldValue { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
    
    public class TenantCustomFieldValueDto
    {
        public string FieldName { get; set; } = string.Empty;
        public string FieldValue { get; set; } = string.Empty;
    }
} 