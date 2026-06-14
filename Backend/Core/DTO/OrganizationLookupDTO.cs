using System;

namespace Core.DTO
{
    public class OrganizationLookupDTO
    {
        public Guid OrganizationId { get; set; }
        public string? OrganizationName { get; set; } = string.Empty;
    }
}