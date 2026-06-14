using System;
using Core.Entities.Enums;

namespace Core.DTO
{
    /// <summary>
    /// DTO cho Thành phần lương người dùng
    /// </summary>
    /// Created by Phuong 08/06/2026
    public class SalaryCompositionDTO
    {
        public Guid? ScId { get; set; }
        public string ScCode { get; set; } = string.Empty;
        public string ScName { get; set; } = string.Empty;
        
        public SalaryCompositionType ScType { get; set; } 
        public SalaryCompositionNature ScNature { get; set; }
        
        public TaxStatus TaxStatus { get; set; }
        public bool IsTaxDeductible { get; set; }
        
        public string? TaxableExpression { get; set; } = string.Empty;
        public string? ExemptExpression { get; set; } = string.Empty;
        public string? LimitExpression { get; set; } = string.Empty;
        public bool AllowExceedLimit { get; set; }
        
        public Entities.Enums.ValueType ValueType { get; set; }
        public CalculationMethod CalculationMethod { get; set; }
        public AggregationScope AggregationScope { get; set; }
        
        public string? CompositionCode { get; set; } = string.Empty;
        public int? OrganizationLevel { get; set; }
        
        public string? FormulaExpression { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        public DisplayOnPayroll IsDisplayedOnPayroll { get; set; }
        
        /// <summary>
        /// Nguồn tạo (0: Hệ thống, 1: Tự thêm)
        /// </summary>
        public SalaryCompositionSource ScSource { get; set; }
        
        /// <summary>
        /// Trạng thái (0: Đang theo dõi, 1: Ngừng theo dõi)
        /// </summary>
        public SalaryStatus ScStatus { get; set; }

        /// <summary>
        /// Danh sách đơn vị áp dụng (Chứa cả ID và Tên)
        /// </summary>
        public List<OrganizationLookupDTO> AppliedOrganizations { get; set; } = [];
    }
}