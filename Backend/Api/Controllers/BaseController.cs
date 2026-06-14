using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
           return Ok(new Response(true, null, 200) { Data = items });
       }

       [HttpGet("{id:guid}")]
       public virtual async Task<ActionResult<Response>> GetById(Guid id)
       {
           var item = await _baseService.GetByIdAsync(id);
           if (item is null) 
               return NotFound(new Response(false, "Không tìm thấy dữ liệu", 404));

           return Ok(new Response(true, null, 200) { Data = item });
       }

       [HttpPost]
       public virtual async Task<ActionResult<Response>> Create([FromBody] TDto dto)
       {
           if (dto is null) 
               return BadRequest(new Response(false, "Dữ liệu không được để trống", 400));

           var newId = await _baseService.CreateAsync(dto);
           if (newId == Guid.Empty) 
               return BadRequest(new Response(false, "Không thể tạo mới bản ghi", 400));

           return StatusCode(201, new Response(true, "Thêm mới thành công", 201) { Data = dto });
       }

       [HttpPut("{id:guid}")]
       public virtual async Task<ActionResult<Response>> Update(Guid id, [FromBody] TDto dto)
       {
           if (dto is null) 
               return BadRequest(new Response(false, "Dữ liệu không được để trống", 400));

           // Lưu ý: Việc gán ID vào DTO nên được xử lý ở Service hoặc DTO phải có sẵn ID
           var updated = await _baseService.UpdateAsync(id, dto);
           if (!updated) 
               return NotFound(new Response(false, "Không tìm thấy dữ liệu để cập nhật", 404));

           return Ok(new Response(true, "Cập nhật thành công", 200) { Data = true });
       }

       [HttpDelete("{id:guid}")]
       public virtual async Task<ActionResult<Response>> Delete(Guid id)
       {
           var deleted = await _baseService.DeleteAsync(id);
           if (!deleted) 
               return NotFound(new Response(false, "Không tìm thấy dữ liệu để xóa", 404));

           return Ok(new Response(true, "Xóa thành công", 200) { Data = true });
       }
    }

}