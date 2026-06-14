using System;
using Core.Entities.Enums;

namespace Core.DTO
{
    /// <summary>
    /// DTO cho Thành phần lương hệ thống
    /// </summary>
    /// Created by Phuong 25/05/2026
    public class SalaryCompositionSystemDTO
    {
        public Guid ScsId { get; set; }
        public string ScsCode { get; set; } = string.Empty;
        public string ScsName { get; set; } = string.Empty;
        public SalaryCompositionType ScsType { get; set; }
        public SalaryCompositionNature ScsNature { get; set; }
        public TaxStatus ScsTaxStatus { get; set; }
        public bool ScsIsTaxDeductible { get; set; }
        public string? ScsTaxableExpression { get; set; } = string.Empty;
        public string? ScsExemptExpression { get; set; } = string.Empty;
        public string? ScsLimitExpression { get; set; } = string.Empty;
        public bool ScsAllowExceedLimit { get; set; }
        public Entities.Enums.ValueType ScsValueType { get; set; }
        public CalculationMethod ScsCalculationMethod { get; set; }
        public AggregationScope ScsAggregationScope { get; set; }
        public string ScsCompositionCode { get; set; } = string.Empty;
        public int? ScsOrganizationLevel { get; set; }
        public string? ScsFormulaExpression { get; set; } = string.Empty;
        public string ScsDescription { get; set; } = string.Empty;
        public DisplayOnPayroll ScsIsDisplayedOnPayroll { get; set; }
        public SalaryCompositionSource ScsSource { get; set; }
        public bool ScsIsInUsed { get; set; }
    }
}