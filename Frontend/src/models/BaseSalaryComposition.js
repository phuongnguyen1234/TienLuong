/**
 * @file Lớp cơ sở cho Thành phần lương.
 * @author Phuong 2026-06-08
 */

class BaseSalaryComposition {
  constructor(data = {}) {
    this.ScCode = data.ScCode || ''
    this.ScName = data.ScName || ''
    this.ScType = data.ScType ?? 0
    this.ScNature = data.ScNature ?? 0
    this.TaxStatus = data.TaxStatus ?? 0
    this.IsTaxDeductible = data.IsTaxDeductible ?? false

    this.TaxableExpression = data.TaxableExpression || null
    this.ExemptExpression = data.ExemptExpression || null
    this.LimitExpression = data.LimitExpression || null
    this.AllowExceedLimit = data.AllowExceedLimit ?? false

    this.ValueType = data.ValueType ?? 0
    this.CalculationMethod = data.CalculationMethod ?? 0
    this.AggregationScope = data.AggregationScope ?? 0

    this.CompositionCode = data.CompositionCode || ''
    this.OrganizationLevel = data.OrganizationLevel || null

    this.FormulaExpression = data.FormulaExpression || null
    this.Description = data.Description || ''
    this.IsDisplayedOnPayroll = data.IsDisplayedOnPayroll ?? 0
  }
}

export default BaseSalaryComposition
