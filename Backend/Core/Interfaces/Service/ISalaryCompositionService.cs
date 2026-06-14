using Core.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.Service
{
    /// <summary>
    /// Interface service cho Thành phần lương
    /// </summary>
    public interface ISalaryCompositionService : IBaseCrudService<SalaryCompositionDTO>
    {
        /// <summary>
        /// Lấy danh sách DTO phân trang
        /// </summary>
        Task<PagedResult<SalaryCompositionDTO>> GetPagingAsync(string? searchTerm, List<string>? filters, int pageIndex, int pageSize);

        /// <summary>
        /// Xóa hàng loạt thành phần lương (xóa mềm)
        /// </summary>
        /// <param name="ids">Danh sách ID cần xóa</param>
        /// <returns>True nếu xóa thành công ít nhất một bản ghi</returns>
        Task<int> BulkDeleteAsync(List<Guid> ids);

        /// <summary>
        /// Cập nhật trạng thái hàng loạt
        /// </summary>
        /// <param name="ids">Danh sách ID</param>
        /// <param name="status">Trạng thái (0: Đang theo dõi, 1: Ngừng theo dõi)</param>
        /// <returns></returns>
        Task<bool> BulkUpdateStatusAsync(List<Guid> ids, int status);

        /// <summary>
        /// Lấy danh sách lookup cho công thức
        /// </summary>
        Task<IEnumerable<SalaryCompositionLookupDTO>> GetLookupsAsync(string? searchTerm);

        /// <summary>
        /// Kiểm tra trùng mã thành phần lương
        /// </summary>
        /// <param name="code">Mã cần check</param>
        /// <param name="excludeId">ID loại trừ</param>
        Task<bool> CheckCodeDuplicateAsync(string code, Guid? excludeId);

        /// <summary>
        /// Sinh mã TPL tự động dựa trên tên
        /// </summary>
        /// <param name="name">Tên TPL</param>
        Task<string> GenerateCodeAsync(string name);
    }
}