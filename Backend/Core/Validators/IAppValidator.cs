using System.Collections.Generic;

namespace Core.Interfaces.Validators
{
    /// <summary>
    /// Interface định nghĩa cấu trúc cho các bộ kiểm tra dữ liệu thủ công
    /// </summary>
    /// <typeparam name="T">Loại DTO cần kiểm tra</typeparam>
    public interface IAppValidator<T>
    {
        /// <summary>
        /// Kiểm tra và trả về danh sách lỗi
        /// </summary>
        Dictionary<string, string> Validate(T dto);

        /// <summary>
        /// Kiểm tra nhanh xem dữ liệu có hợp lệ hay không
        /// </summary>
        bool IsValid(T dto, out Dictionary<string, string> errors);
    }
}