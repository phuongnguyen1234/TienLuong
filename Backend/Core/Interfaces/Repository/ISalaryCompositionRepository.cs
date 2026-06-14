using Core.Entities;
using Core.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.Repository
{
    /// <summary>
    /// Interface repository cho Thành phần lương
    /// </summary>
    public interface ISalaryCompositionRepository : IBaseRepository<SalaryComposition>
    {
        /// <summary>
        /// Lấy danh sách phân trang kèm theo dữ liệu mapping đơn vị để Service lắp ghép DTO
        /// </summary>
        Task<(IEnumerable<SalaryComposition> Items, IEnumerable<OrganizationMapping> Mappings, long TotalCount)> GetPagingAsync(string? searchTerm, List<FilterCriteria>? filters, int pageIndex, int pageSize);

        /// <summary>
        /// Thêm mới TPL kèm theo danh sách đơn vị áp dụng (trong cùng một giao dịch)
        /// </summary>
        Task<Guid> CreateWithOrganizationsAsync(SalaryComposition entity, List<Guid> organizationIds);

        /// <summary>
        /// Lấy chi tiết TPL và danh sách thông tin đơn vị áp dụng (Lookup)
        /// </summary>
        Task<(SalaryComposition? Entity, List<OrganizationLookupDTO> Organizations)> GetByIdWithOrganizationsAsync(Guid id);

        /// <summary>
        /// Cập nhật TPL và đồng bộ lại danh sách đơn vị áp dụng
        /// </summary>
        Task<bool> UpdateWithOrganizationsAsync(SalaryComposition entity, List<Guid> organizationIds);

        /// <summary>
        /// Xóa hàng loạt TPL (xóa mềm) dựa trên danh sách ID
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<int> BulkDeleteAsync(List<Guid> ids);

        /// <summary>
        /// Cập nhật trạng thái hàng loạt (Đang theo dõi/Ngừng theo dõi)
        /// </summary>
        /// <param name="ids">Danh sách ID</param>
        /// <param name="status">Trạng thái mới</param>
        /// <returns></returns>
        Task<bool> BulkUpdateStatusAsync(List<Guid> ids, int status);

        /// <summary>
        /// Lấy danh sách rút gọn (ID, Code, Name) để làm lookup
        /// </summary>
        Task<IEnumerable<SalaryCompositionLookupDTO>> GetLookupsAsync(string? searchTerm);

        /// <summary>
        /// Kiểm tra trùng mã TPL
        /// </summary>
        /// <param name="code">Mã cần kiểm tra</param>
        /// <param name="excludeId">ID bản ghi loại trừ (khi sửa)</param>
        Task<bool> CheckCodeDuplicateAsync(string code, Guid? excludeId);
    }
}