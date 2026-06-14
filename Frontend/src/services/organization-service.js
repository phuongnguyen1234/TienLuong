import apiClient from './axios-client'
import { BaseService } from './base-service'

/**
 * Service xử lý các nghiệp vụ liên quan đến Đơn vị công tác.
 * Kế thừa các hàm CRUD cơ bản từ BaseService.
 */
class OrganizationService extends BaseService {
  constructor() {
    super('Organizations') // Khai báo endpoint cho controller Organizations
  }

  /**
   * Lấy danh sách đơn vị công tác theo cấu trúc cây
   * @returns {Promise<Array>} Danh sách đơn vị công tác dạng cây
   */
  async getOrganizationTree() {
    return await apiClient.get(`${this.controllerEndpoint}/tree`)
  }
}

export default new OrganizationService()
