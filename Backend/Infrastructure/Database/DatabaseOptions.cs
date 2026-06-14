using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    /// <summary>
    /// Lớp này dùng để lưu trữ các tùy chọn cấu hình cho kết nối cơ sở dữ liệu, như chuỗi kết nối.
    /// Các tùy chọn này sẽ được đọc từ tệp cấu hình (ví dụ: appsettings.json) 
    /// và được sử dụng để thiết lập kết nối đến cơ sở dữ liệu trong ứng dụng.
    /// </summary>
    /// Created by Phuong 25/02/2026
    public class DatabaseOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
    }
}
