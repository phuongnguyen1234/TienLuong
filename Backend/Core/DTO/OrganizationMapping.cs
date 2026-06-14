namespace Core.DTO
{
    /// <summary>
    /// Class trung gian dùng để chứa kết quả ánh xạ từ Database phục vụ lắp ghép DTO ở Service
    /// </summary>
    public class OrganizationMapping 
    {
        public Guid ScId { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
    }
}