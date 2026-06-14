using Core.Entities;
using Core.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.Repository
{
    /// <summary>
    /// Interface repository cho Thành phần lương hệ thống
    /// </summary>
    public interface ISalaryCompositionSystemRepository : IBaseRepository<SalaryCompositionSystem>
    {
        /// <summary>
        /// Lấy danh sách phân trang và lọc cho TPL hệ thống
        /// </summary>
        Task<(IEnumerable<SalaryCompositionSystem> Items, long TotalCount)> GetPagingAsync(string? searchTerm, List<FilterCriteria>? filters, int pageIndex, int pageSize);

        /// <summary>
        /// Sao chép hàng loạt TPL hệ thống sang TPL người dùng
        /// </summary>
        /// <param name="systemIds">Danh sách ID TPL hệ thống</param>
        Task<int> BulkCloneAsync(List<Guid> systemIds);
    }
}