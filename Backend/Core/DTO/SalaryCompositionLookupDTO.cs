using System;

namespace Core.DTO
{
    /// <summary>
    /// DTO rút gọn cho Thành phần lương phục vụ việc chọn trong công thức
    /// </summary>
    public class SalaryCompositionLookupDTO
    {
        public Guid ScId { get; set; }
        public string ScCode { get; set; } = string.Empty;
        public string ScName { get; set; } = string.Empty;
    }
}