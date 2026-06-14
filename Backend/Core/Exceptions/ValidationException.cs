using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Exceptions
{
    /// <summary>
    /// Lỗi valid dữ liệu
    /// </summary>
    /// Created by Phuong 25/02/2026
    public class ValidationException : Exception
    {
        /// <summary>
        /// Dữ liệu lỗi
        /// </summary>
        IDictionary _data = new Dictionary<string, string>();

        /// <summary>
        /// Thông báo lỗi
        /// </summary>
        private string? _message = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin";

        public ValidationException(IDictionary data)
        {
            _data = data;
        }

        public override string Message => _message = string.Empty;
        public override IDictionary Data => _data;
    }
}
