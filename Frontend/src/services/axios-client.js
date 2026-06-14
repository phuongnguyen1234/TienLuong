import axios from 'axios'

// Cấu hình đường dẫn gốc cho API
// Hãy thay đổi port (ví dụ: 7166) phù hợp với launchSettings.json của Backend
export const BASE_URL = 'https://localhost:7109'

const apiClient = axios.create({
  baseURL: `${BASE_URL}/api/`,
  headers: {
    'Content-Type': 'application/json',
  },
  // Thời gian chờ tối đa (10s)
  //timeout: 10000,
})

// Response Interceptor: Tự động bóc tách dữ liệu từ wrapper Response của Backend
apiClient.interceptors.response.use(
  (response) => {
    // Khi Backend trả về PascalCase, IsSuccess sẽ có giá trị true
    if (response.data && response.data.IsSuccess) {
      return response.data.Data
    }
    return response.data
  },
  (error) => {
    // Xử lý lỗi tập trung (Ví dụ: show toast thông báo lỗi)
    const message = error.response?.data?.Message || 'Có lỗi xảy ra, vui lòng thử lại.'
    console.error('API Error:', message)
    return Promise.reject(error)
  },
)

export default apiClient
