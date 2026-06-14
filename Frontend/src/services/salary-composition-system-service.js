import { BaseService } from './base-service'
import apiClient from './axios-client'
import SalaryCompositionSystem from '@/models/SalaryCompositionSystem'

/**
 * Service xử lý các nghiệp vụ liên quan đến Thành phần lương hệ thống.
 * Cung cấp các phương thức để lấy danh mục mẫu và thực hiện đưa vào sử dụng.
 */
class SalaryCompositionSystemService extends BaseService {
  constructor() {
    super('SalaryCompositionSystems')
  }

  /**
   * Lấy danh sách TPL hệ thống có phân trang, tìm kiếm và lọc.
   * @param {string} searchTerm - Từ khóa tìm kiếm (Mã hoặc Tên)
   * @param {Array<string>} filters - Mảng các tiêu chí lọc (Ví dụ: "ScsType:Equals:5")
   * @param {number} pageIndex - Trang hiện tại (bắt đầu từ 1)
   * @param {number} pageSize - Số bản ghi trên mỗi trang
   */
  async getPaging(searchTerm = '', filters = [], pageIndex = 1, pageSize = 15) {
    const response = await super.getPaging(searchTerm, filters, pageIndex, pageSize)

    // Chuyển đổi mảng dữ liệu thô từ API thành mảng các đối tượng Model
    if (response && response.Items) {
      response.Items = response.Items.map((item) => new SalaryCompositionSystem(item))
    }
    return response
  }

  /**
   * Đưa hàng loạt thành phần lương hệ thống vào danh sách sử dụng (Clone).
   * @param {Array<string>} ids - Danh sách các GUID của TPL hệ thống được chọn.
   * @returns {Promise<number>} Số lượng bản ghi đã được clone thành công (đã qua interceptor bóc tách).
   */
  async bulkClone(ids) {
    return await apiClient.post(`${this.controllerEndpoint}/bulk-clone`, ids)
  }
}

export default new SalaryCompositionSystemService()
