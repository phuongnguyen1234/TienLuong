using Core.DTO;
using Core.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    /// <summary>
    /// Controller cơ sở chứa các cấu hình chung
    /// </summary>
    /// Created by Phuong 24/05/2026
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
    }

    /// <summary>
    /// Controller Generic xử lý các thao tác CRUD cơ bản
    /// </summary>
    /// <typeparam name="TDto">DTO dùng chung cho thêm/sửa/xóa</typeparam>
    public abstract class BaseCrudController<TDto> : BaseController
    {
       protected readonly IBaseCrudService<TDto> _baseService;

       protected BaseCrudController(IBaseCrudService<TDto> baseService)
       {
           _baseService = baseService;
       }

       [HttpGet]
       public virtual async Task<ActionResult<Response>> GetAll()
       {
           var items = await _baseService.GetAllAsync();
           return Success(items);
       }

       [HttpGet("{id:guid}")]
       public virtual async Task<ActionResult<Response>> GetById(Guid id)
       {
           var item = await _baseService.GetByIdAsync(id);
           if (item is null) 
               return Failure("Không tìm thấy dữ liệu", 404);

           return Success(item);
       }

       [HttpPost]
       public virtual async Task<ActionResult<Response>> Create([FromBody] TDto dto)
       {
           if (dto is null) 
               return Failure("Dữ liệu không được để trống");

           var newId = await _baseService.CreateAsync(dto);
           if (newId == Guid.Empty) 
               return Failure("Không thể tạo mới bản ghi");

           return Success(dto, "Thêm mới thành công", 201);
       }

       [HttpPut("{id:guid}")]
       public virtual async Task<ActionResult<Response>> Update(Guid id, [FromBody] TDto dto)
       {
           if (dto is null) 
               return Failure("Dữ liệu không được để trống");

           // Lưu ý: Việc gán ID vào DTO nên được xử lý ở Service hoặc DTO phải có sẵn ID
           var updated = await _baseService.UpdateAsync(id, dto);
           if (!updated) 
               return Failure("Không tìm thấy dữ liệu để cập nhật", 404);

           return Success(true, "Cập nhật thành công");
       }

       [HttpDelete("{id:guid}")]
       public virtual async Task<ActionResult<Response>> Delete(Guid id)
       {
           var deleted = await _baseService.DeleteAsync(id);
           if (!deleted) 
               return Failure("Không tìm thấy dữ liệu để xóa", 404);

           return Success(true, "Xóa thành công");
       }

        protected ActionResult<Response> Success(object? data = null, string? message = null, int statusCode = 200)
        {
            return StatusCode(statusCode, new Response(true, message, statusCode) { Data = data });
        }

        protected ActionResult<Response> Failure(string message, int statusCode = 400)
        {
            return StatusCode(statusCode, new Response(false, message, statusCode));
        }
    }

}