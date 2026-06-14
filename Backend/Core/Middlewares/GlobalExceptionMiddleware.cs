using Core.DTO;
using Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Core.Middlewares
{
    /// <summary>
    /// Middleware xử lý exception
    /// </summary>
    /// Created by Phuong 25/02/2026
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException vex)
            {
                // nếu lỗi validate thì ném lỗi 400
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                var response = new Response(
                    success: false, 
                    message: "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.", 
                    statusCode: StatusCodes.Status400BadRequest);
                
                response.Data = vex.Data; // Gán dữ liệu lỗi chi tiết vào Data

                var json = JsonConvert.SerializeObject(response);
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                // nếu lỗi chung chung của server thì ném lỗi 500
                var response = new Response(false, "Đã có lỗi xảy ra, vui lòng liên hệ MISA để được hỗ trợ.", 500);
                response.DevMessage = ex.Message;
                #if DEBUG
                response.DevMessage = ex.ToString();
                #endif

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var json = JsonConvert.SerializeObject(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
