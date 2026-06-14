using Core.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.Service
{
    /// <summary>
    /// Interface service cho Thành phần lương hệ thống
    /// </summary>
    public interface ISalaryCompositionSystemService : IBaseCrudService<SalaryCompositionSystemDTO>
    {
        /// <summary>
        /// Lấy danh sách TPL hệ thống có phân trang và lọc
        /// </summary>
        Task<PagedResult<SalaryCompositionSystemDTO>> GetPagingAsync(string? searchTerm, List<string>? filters, int pageIndex, int pageSize);

        /// <summary>
        /// Đưa hàng loạt TPL hệ thống vào sử dụng
        /// </summary>
        /// <param name="systemIds">Danh sách ID hệ thống</param>
        Task<int> BulkCloneAsync(List<Guid> systemIds);
    }
}