/**
 * @file Định nghĩa các Enum cho Thành phần lương.
 * @author Phuong 2026-06-08
 */

export const SalaryCompositionType = Object.freeze({
  EmployeeInfo: 0,
  Timekeeping: 1,
  Sales: 2,
  KPI: 3,
  Production: 4,
  Salary: 5,
  PersonalIncomeTax: 6,
  InsuranceAndUnion: 7,
  Other: 8,
})

export const SalaryCompositionNature = Object.freeze({
  Income: 0,
  Deduction: 1,
  Other: 2,
})

export const TaxStatus = Object.freeze({
  Taxable: 0,
  FullyExempt: 1,
  PartiallyExempt: 2,
})

export const ValueType = Object.freeze({
  Currency: 0,
  Number: 1,
  Text: 2,
  Date: 3,
  Percentage: 4,
})

export const CalculationMethod = Object.freeze({
  AutoSum: 0,
  Formula: 1,
})

export const AggregationScope = Object.freeze({
  SameOrganization: 0,
  Subordinates: 1,
  Structure: 2,
})

export const DisplayOnPayroll = Object.freeze({
  No: 0,
  Yes: 1,
  OnlyIfNonZero: 2,
})

export const SalaryStatus = Object.freeze({
  Active: 0,
  Inactive: 1,
})

export const SalaryCompositionSource = Object.freeze({
  SystemDefault: 0,
  UserAdded: 1,
})
