using System;

namespace Core.DTO
{
    /// <summary>
    /// DTO cho Đơn vị công tác
    /// </summary>
    /// Created by Phuong 25/05/2026
    public class OrganizationDTO
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public Guid? OrganizationParentId { get; set; }
        public List<OrganizationDTO> Children { get; set; } = new List<OrganizationDTO>();
    }
}