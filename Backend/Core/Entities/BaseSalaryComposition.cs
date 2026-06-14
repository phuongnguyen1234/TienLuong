using Core.Entities.Enums;

namespace Core.Entities
{
    /// <summary>
    /// Lớp cơ sở chứa các thuộc tính dùng chung cho cả TPL Hệ thống và TPL Người dùng
    /// </summary>
    public abstract class BaseSalaryComposition
    {
        public string ScCode { get; set; } = string.Empty;
        public string ScName { get; set; } = string.Empty;
        
        public SalaryCompositionType ScType { get; set; } 
        
        public SalaryCompositionNature ScNature { get; set; }
        public TaxStatus ScTaxStatus { get; set; }
        public bool ScIsTaxDeductible { get; set; } // tinyint (0/1) -> bool
        
        public string? ScTaxableExpression { get; set; } = string.Empty;
        public string? ScExemptExpression { get; set; } = string.Empty;
        public string? ScLimitExpression { get; set; } = string.Empty;
        public bool ScAllowExceedLimit { get; set; }
        
        public Enums.ValueType ScValueType { get; set; }
        public CalculationMethod ScCalculationMethod { get; set; }
        public AggregationScope ScAggregationScope { get; set; }
        
        public string ScCompositionCode { get; set; } = string.Empty;
        public int? ScOrganizationLevel { get; set; }
        
        public string? ScFormulaExpression { get; set; } = string.Empty;
        public string ScDescription { get; set; } = string.Empty;
        
        public DisplayOnPayroll ScIsDisplayedOnPayroll { get; set; }
        public bool ScIsDeleted { get; set; } // tinyint (0/1) -> bool
    }
}