using Core.DTO;
using Core.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers
{
    public class SalaryCompositionSystemsController : BaseCrudController<SalaryCompositionSystemDTO>
    {
        private readonly ISalaryCompositionSystemService _systemService;

        public SalaryCompositionSystemsController(ISalaryCompositionSystemService service) : base(service)
        {
            _systemService = service;
        }

        /// <summary>
        /// Lấy danh sách TPL hệ thống có phân trang và lọc
        /// </summary>
        [HttpGet("paging")]
        public async Task<ActionResult<Response>> GetPaging(
            [FromQuery] string? searchTerm, 
            [FromQuery] List<string>? filters, 
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 15)
        {
            var result = await _systemService.GetPagingAsync(searchTerm, filters, pageIndex, pageSize);
            return Success(result);
        }

        /// <summary>
        /// Đưa hàng loạt TPL hệ thống vào sử dụng (Clone)
        /// </summary>
        [HttpPost("bulk-clone")]
        public async Task<ActionResult<Response>> BulkClone([FromBody] List<Guid> ids)
        {
            var result = await _systemService.BulkCloneAsync(ids);
            var message = $"Đã đưa thành công {result} thành phần lương vào sử dụng.";
            return Success(result, message);
        }

        /// <summary>
        /// Lấy cấu hình cột của grid
        /// </summary>
        [HttpGet("grid-config/{gridKey}")]
        public async Task<ActionResult<Response>> GetGridConfig([FromRoute] string gridKey)
        {
            var result = await _systemService.GetGridConfigAsync(gridKey);
            return Success(result);
        }

        /// <summary>
        /// Lưu cấu hình cột của grid (Thêm mới hoặc Cập nhật)
        /// </summary>
        [HttpPost("grid-config")]
        public async Task<ActionResult<Response>> SaveGridConfig([FromBody] GridConfigDTO configDto)
        {
            var result = await _systemService.SaveGridConfigAsync(configDto);
            return Success(result, "Lưu cấu hình thành công");
        }
    }
}