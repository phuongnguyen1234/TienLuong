/**
 * @file Lớp Thành phần lương hệ thống.
 * @author Phuong 2026-06-08
 */

import BaseSalaryComposition from './BaseSalaryComposition'

class SalaryCompositionSystem extends BaseSalaryComposition {
  constructor(data = {}) {
    // Ánh xạ PascalCase từ Server (Scs...) sang các thuộc tính mà BaseSalaryComposition mong đợi
    const baseData = {
      ...data,
      ScCode: data.ScsCode || '',
      ScName: data.ScsName || '',
      ScType: data.ScsType,
      ScNature: data.ScsNature,
      TaxStatus: data.ScsTaxStatus,
      IsTaxDeductible: data.ScsIsTaxDeductible,
      TaxableExpression: data.ScsTaxableExpression,
      ExemptExpression: data.ScsExemptExpression,
      LimitExpression: data.ScsLimitExpression,
      AllowExceedLimit: data.ScsAllowExceedLimit,
      ValueType: data.ScsValueType,
      CalculationMethod: data.ScsCalculationMethod,
      AggregationScope: data.ScsAggregationScope,
      CompositionCode: data.ScsCompositionCode,
      OrganizationLevel: data.ScsOrganizationLevel,
      FormulaExpression: data.ScsFormulaExpression,
      Description: data.ScsDescription,
      IsDisplayedOnPayroll: data.ScsIsDisplayedOnPayroll,
    }

    super(baseData)

    // Thuộc tính định danh riêng của System
    this.ScsId = data.ScsId || null
    this.ScsIsInUsed = data.ScsIsInUsed ?? false
  }
}

export default SalaryCompositionSystem
