/**
 * @file Định nghĩa lớp SalaryComposition cho thành phần lương.
 * @author Phuong 2026-06-08
 */

import BaseSalaryComposition from './BaseSalaryComposition'

/**
 * Lớp đại diện cho một Thành phần lương trong hệ thống.
 * Các thuộc tính được ánh xạ từ SalaryCompositionDTO.cs
 */
class SalaryComposition extends BaseSalaryComposition {
  /**
   * Khởi tạo một đối tượng SalaryComposition mới.
   * @param {Object} data - Dữ liệu để khởi tạo đối tượng.
   */
  constructor(data = {}) {
    super(data)
    this.ScId = data.ScId || null
    this.ScSource = data.ScSource ?? 1
    this.ScStatus = data.ScStatus ?? 0
    this.AppliedOrganizations = data.AppliedOrganizations || []
  }
}

export default SalaryComposition
