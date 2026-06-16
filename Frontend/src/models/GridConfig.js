/**
 * Model cho cấu hình Grid
 */
export default class GridConfig {
  constructor(data = {}) {
    // Khóa của Grid (ví dụ: pa_salary_composition)
    this.gridKey = data.gridKey || ''
    // Dữ liệu cấu hình (JSON string)
    this.configData = data.configData || ''
  }
}
