using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    /// <summary>
    /// DTO trả về của controller
    /// </summary>
    /// Created by Phuong 26/02/2026
    public class Response
    {
        /// <summary>
        /// Có thành công không
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Thông báo
        /// </summary>
        public string? UserMessage { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// Thông báo cho lập trình viên (lỗi chi tiết, stack trace...)
        /// </summary>
        public string? DevMessage { get; set; }

        /// <summary>
        /// Dữ liệu trả về (Object, List, PagedResult...)
        /// </summary>
        public object? Data { get; set; }

        public Response() { }
        public Response(bool success, string? message, int? statusCode, object? data = null, string? devMessage = null)
        {
            IsSuccess = success;
            UserMessage = message;
            StatusCode = statusCode;
            Data = data;
            DevMessage = devMessage;
        }
    }
}
