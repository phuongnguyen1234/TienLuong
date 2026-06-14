using Core.DTO;
using Core.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Controller xử lý Thành phần lương người dùng
    /// </summary>
    public class SalaryCompositionsController : BaseCrudController<SalaryCompositionDTO>
    {
        private readonly ISalaryCompositionService _salaryService;

        public SalaryCompositionsController(ISalaryCompositionService salaryService) : base(salaryService)
        {
            _salaryService = salaryService;
        }

        /// <summary>
        /// Lấy danh sách phân trang và tìm kiếm
        /// </summary>
        [HttpGet("paging")]
        public async Task<ActionResult<Response>> GetPaging(
            [FromQuery] string? searchTerm, 
            [FromQuery] List<string>? filters, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 15)
        {
            var result = await _salaryService.GetPagingAsync(searchTerm, filters, pageIndex, pageSize);
            return Ok(new Response(true, null, 200, result));
        }

        /// <summary>
        /// Xóa hàng loạt thành phần lương (Xóa mềm)
        /// </summary>
        /// <param name="ids">Danh sách ID cần xóa</param>
        [HttpDelete("bulk")]
        public async Task<ActionResult<Response>> BulkDelete([FromBody] List<Guid> ids)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest(new Response(false, "Danh sách ID không được để trống", 400));

            var result = await _salaryService.BulkDeleteAsync(ids);
            return Ok(new Response(true, "Xóa hàng loạt thành công", 200, result));
        }

        /// <summary>
        /// Cập nhật trạng thái hàng loạt (Ngừng/Tiếp tục theo dõi)
        /// </summary>
        [HttpPut("bulk-status")]
        public async Task<ActionResult<Response>> BulkUpdateStatus([FromBody] List<Guid> ids, [FromQuery] int status)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest(new Response(false, "Danh sách ID không được để trống", 400));

            var result = await _salaryService.BulkUpdateStatusAsync(ids, status);
            string message = status == 0 ? "Tiếp tục theo dõi thành công" : "Ngừng theo dõi thành công";
            return Ok(new Response(true, message, 200, result));
        }

        /// <summary>
        /// Lấy danh sách TPL rút gọn phục vụ chọn trong công thức
        /// </summary>
        [HttpGet("lookup")]
        public async Task<ActionResult<Response>> GetLookup([FromQuery] string? searchTerm)
        {
            var result = await _salaryService.GetLookupsAsync(searchTerm);
            return Ok(new Response(true, null, 200, result));
        }

        /// <summary>
        /// Kiểm tra mã TPL đã tồn tại hay chưa
        /// </summary>
        [HttpGet("check-duplicate")]
        public async Task<ActionResult<Response>> CheckDuplicate([FromQuery] string code, [FromQuery] Guid? excludeId)
        {
            var isDuplicate = await _salaryService.CheckCodeDuplicateAsync(code, excludeId);
            return Ok(new Response(true, null, 200, isDuplicate));
        }

        /// <summary>
        /// Sinh mã TPL từ tên (UPPER_SNAKE_CASE)
        /// </summary>
        [HttpGet("generate-code")]
        public async Task<ActionResult<Response>> GenerateCode([FromQuery] string name)
        {
            var code = await _salaryService.GenerateCodeAsync(name);
            return Ok(new Response(true, null, 200, code));
        }
    }
}