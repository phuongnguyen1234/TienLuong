import { BaseService } from './base-service'
import apiClient from './axios-client'

/**
 * Service xử lý các nghiệp vụ liên quan đến Thành phần lương.
 * Kế thừa các hàm CRUD cơ bản từ BaseService.
 */
class SalaryCompositionService extends BaseService {
  constructor() {
    super('SalaryCompositions')
  }

  /**
   * Tạo mới thành phần lương.
   * Chuyển đổi dữ liệu từ form (snake_case) sang DTO (camelCase) cho Backend.
   * @param {Object} data - Dữ liệu từ form
   */
  async create(data) {
    const dto = this._mapToDto(data)
    return await super.create(dto)
  }

  /**
   * Cập nhật thành phần lương.
   * @param {string} id - GUID của thành phần lương
   * @param {Object} data - Dữ liệu từ form
   */
  async update(id, data) {
    const dto = this._mapToDto(data)
    dto.scId = id
    return await super.update(id, dto)
  }

  /**
   * Lấy danh sách phân trang.
   */
  async getPaging(searchTerm = '', filters = [], pageIndex = 1, pageSize = 15) {
    // Các component đã gửi đúng định dạng "Key:Op:Value", không cần replace dấu phẩy nữa
    return await super.getPaging(searchTerm, filters, pageIndex, pageSize)
  }

  /**
   * Xóa hàng loạt thành phần lương
   * @param {Array<string>} ids - Danh sách GUID
   */
  async bulkDelete(ids) {
    return await apiClient.delete(`${this.controllerEndpoint}/bulk`, { data: ids })
  }

  /**
   * Cập nhật trạng thái hàng loạt (Ngừng/Tiếp tục theo dõi)
   * @param {Array<string>} ids - Danh sách GUID
   * @param {number} status - 0: Đang theo dõi, 1: Ngừng theo dõi
   */
  async bulkUpdateStatus(ids, status) {
    return await apiClient.put(`${this.controllerEndpoint}/bulk-status`, ids, {
      params: { status },
    })
  }

  /**
   * Lấy danh sách rút gọn để phục vụ chọn trong công thức
   * @param {string} searchTerm - Từ khóa tìm kiếm
   */
  async getLookup(searchTerm = '') {
    return await apiClient.get(`${this.controllerEndpoint}/lookup`, {
      params: { searchTerm },
    })
  }

  /**
   * Kiểm tra mã thành phần lương đã tồn tại hay chưa
   * @param {string} code - Mã cần check
   * @param {string} excludeId - ID loại trừ (khi sửa)
   */
  async checkDuplicate(code, excludeId = null) {
    return await apiClient.get(`${this.controllerEndpoint}/check-duplicate`, {
      params: { code, excludeId },
    })
  }

  /**
   * Gọi Server sinh mã tự động theo tên (UPPER_SNAKE_CASE)
   * @param {string} name - Tên thành phần lương
   */
  async generateCode(name) {
    return await apiClient.get(`${this.controllerEndpoint}/generate-code`, {
      params: { name },
    })
  }

  /**
   * Helper ánh xạ dữ liệu form (snake_case) sang DTO (camelCase) theo chuẩn Backend.
   * @param {Object} data - Dữ liệu thô từ form
   * @private
   */
  _mapToDto(data) {
    // Determine if input data uses snake_case (from form) or PascalCase (from existing DTO/backend)
    const isSnakeCaseInput = Object.prototype.hasOwnProperty.call(data, 'sc_code')

    const getVal = (snakeKey, pascalKey, defaultValue = null) => {
      const value = isSnakeCaseInput ? data[snakeKey] : data[pascalKey]
      // For string fields, ensure they are never null, convert to empty string if null/undefined
      if (typeof value === 'string' && (value === null || value === undefined)) {
        return ''
      }
      return value ?? defaultValue
    }

    const getBoolVal = (snakeKey, pascalKey) => {
      const val = isSnakeCaseInput ? data[snakeKey] : data[pascalKey]
      return !!val // Convert to boolean (0/1 or true/false to boolean)
    }

    const getAppliedOrganizations = () => {
      if (isSnakeCaseInput) {
        return (data.organization_ids || []).map((id) => ({
          organizationId: id,
          organizationName: null, // Backend sẽ tự resolve tên dựa trên ID nếu cần
        }))
      } else {
        // Nếu input là PascalCase, giả định AppliedOrganizations đã ở định dạng DTO chính xác
        return data.AppliedOrganizations || []
      }
    }

    return {
      scId: getVal('sc_id', 'ScId'),
      scCode: getVal('sc_code', 'ScCode', ''),
      scName: getVal('sc_name', 'ScName', ''),
      scType: getVal('sc_type', 'ScType'),
      scNature: getVal('sc_nature', 'ScNature'),
      taxStatus: getVal('sc_tax_status', 'TaxStatus', 0),
      isTaxDeductible: getBoolVal('sc_is_tax_deductible', 'IsTaxDeductible'),
      taxableExpression: getVal('sc_taxable_expression', 'TaxableExpression', ''),
      exemptExpression: getVal('sc_exempt_expression', 'ExemptExpression', ''),
      limitExpression: getVal('sc_limit_expression', 'LimitExpression', ''),
      allowExceedLimit: getBoolVal('sc_allow_exceed_limit', 'AllowExceedLimit'),
      valueType: getVal('sc_value_type', 'ValueType'),
      calculationMethod: getVal('sc_calculation_method', 'CalculationMethod'),
      aggregationScope: getVal('sc_aggregation_scope', 'AggregationScope'),
      compositionCode: getVal('sc_composition_code', 'CompositionCode', ''),
      organizationLevel: getVal('sc_organization_level', 'OrganizationLevel'),
      formulaExpression: getVal('sc_formula_expression', 'FormulaExpression', ''),
      description: getVal('sc_description', 'Description', ''),
      isDisplayedOnPayroll: getVal('sc_is_displayed_on_payroll', 'IsDisplayedOnPayroll'),
      scSource: getVal('sc_source', 'ScSource'),
      scStatus: getVal('sc_status', 'ScStatus', 0),
      // Chuyển đổi mảng IDs đơn vị sang mảng Objects theo yêu cầu của API
      appliedOrganizations: getAppliedOrganizations(),
    }
  }
}

export default new SalaryCompositionService()
