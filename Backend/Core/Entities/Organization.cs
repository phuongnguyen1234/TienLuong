using System;

namespace Core.Entities
{
    /// <summary>
    /// Thực thể Đơn vị công tác
    /// </summary>
    /// Created by Phuong 25/05/2026
    public class Organization
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public Guid? OrganizationParentId { get; set; }
    }
}