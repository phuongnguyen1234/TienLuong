/**
 * @file Các hàm tiện ích để chuyển đổi giá trị enum của SalaryComposition sang văn bản hiển thị.
 * @author Phuong 2026-06-08
 */

import {
  SalaryCompositionType,
  SalaryCompositionNature,
  TaxStatus,
  ValueType,
  CalculationMethod,
  AggregationScope,
  SalaryCompositionSource,
  DisplayOnPayroll,
  SalaryStatus,
} from '@/models/SalaryEnums'

export function getTypeName(type) {
  const map = {
    [SalaryCompositionType.EmployeeInfo]: 'Thông tin nhân viên',
    [SalaryCompositionType.Timekeeping]: 'Chấm công',
    [SalaryCompositionType.Sales]: 'Doanh số',
    [SalaryCompositionType.KPI]: 'KPI',
    [SalaryCompositionType.Production]: 'Sản phẩm',
    [SalaryCompositionType.Salary]: 'Lương, tiền công',
    [SalaryCompositionType.PersonalIncomeTax]: 'Thuế TNCN',
    [SalaryCompositionType.InsuranceAndUnion]: 'Bảo hiểm - công đoàn',
    [SalaryCompositionType.Other]: 'Khác',
  }

  return map[type] || 'Không xác định'
}

export function getNatureText(nature) {
  const map = {
    [SalaryCompositionNature.Income]: 'Thu nhập',
    [SalaryCompositionNature.Deduction]: 'Khấu trừ',
    [SalaryCompositionNature.Other]: 'Khác',
  }
  return map[nature] || 'Không xác định'
}

export function getTaxStatusText(taxStatus) {
  const map = {
    [TaxStatus.Taxable]: 'Chịu thuế',
    [TaxStatus.FullyExempt]: 'Miễn thuế toàn phần',
    [TaxStatus.PartiallyExempt]: 'Miễn thuế một phần',
  }
  return map[taxStatus] || 'Không xác định'
}

export function getValueTypeText(valueType) {
  const map = {
    [ValueType.Currency]: 'Tiền tệ',
    [ValueType.Number]: 'Số',
    [ValueType.Text]: 'Chữ',
    [ValueType.Date]: 'Ngày',
    [ValueType.Percentage]: 'Phần trăm',
  }
  return map[valueType] || 'Không xác định'
}

export function getCalculationMethodText(method) {
  const map = {
    [CalculationMethod.AutoSum]: 'Tự động cộng tổng',
    [CalculationMethod.Formula]: 'Tính theo công thức',
  }
  return map[method] || 'Không xác định'
}

export function getAggregationScopeText(scope) {
  const map = {
    [AggregationScope.SameOrganization]: 'Trong cùng đơn vị công tác',
    [AggregationScope.Subordinates]: 'Dưới quyền',
    [AggregationScope.Structure]: 'Thuộc cơ cấu tổ chức',
  }
  return map[scope] || 'Không xác định'
}

export function getSourceText(source) {
  const map = {
    [SalaryCompositionSource.SystemDefault]: 'Mặc định',
    [SalaryCompositionSource.UserAdded]: 'Tự thêm',
  }
  return map[source] || 'Không xác định'
}

export function getIsDisplayedOnPayrollText(status) {
  const map = {
    [DisplayOnPayroll.No]: 'Không',
    [DisplayOnPayroll.Yes]: 'Có',
    [DisplayOnPayroll.OnlyIfNonZero]: 'Chỉ hiển thị nếu khác 0',
  }
  return map[status] || 'Không xác định'
}

export function getStatusText(status) {
  const map = {
    [SalaryStatus.Active]: 'Đang theo dõi',
    [SalaryStatus.Inactive]: 'Ngừng theo dõi',
  }
  return map[status] || 'Không xác định'
}
