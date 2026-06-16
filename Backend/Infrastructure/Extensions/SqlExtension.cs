using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Infrastructure.Extensions
{
    /// <summary>
    /// Lớp tiện ích quản lý và truy xuất các câu lệnh SQL từ tệp cấu hình JSON.
    /// Cơ chế này giúp tách biệt mã C# và mã SQL, dễ dàng bảo trì và tối ưu hóa câu lệnh mà không cần compile lại source code.
    /// </summary>
    /// <remarks>
    /// Các tệp JSON phải được đặt trong thư mục "Queries" tại thư mục thực thi và có định dạng tên: {EntityName}Queries.json.
    /// </remarks>
    /// Created by Phuong 24/05/2026
    public static class SqlExtension
    {
        /// <summary>
        /// Bộ nhớ đệm (Cache) lưu trữ các câu truy vấn đã được tải lên từ đĩa.
        /// Cấu trúc: Dictionary[EntityName, Dictionary[QueryKey, QueryString]]
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, string>> _queries = [];

        /// <summary>
        /// Lấy nội dung câu lệnh SQL dựa trên tên thực thể và từ khóa của query.
        /// </summary>
        /// <param name="entityName">Tên thực thể (ví dụ: SalaryComposition, Organization).</param>
        /// <param name="queryKey">Từ khóa định danh câu lệnh trong file JSON (ví dụ: GetPaging, Insert).</param>
        /// <returns>Chuỗi câu lệnh SQL hoặc tên Stored Procedure.</returns>
        /// <exception cref="KeyNotFoundException">Ném ra khi không tìm thấy tệp JSON hoặc từ khóa query tương ứng.</exception>
        public static string GetQuery(string entityName, string queryKey)
        {
            // Kiểm tra xem queries của entity này đã được load vào cache chưa
            if (!_queries.TryGetValue(entityName, out var entityQueries))
            {
                LoadQueries(entityName);
                _queries.TryGetValue(entityName, out entityQueries);
            }

            // Kiểm tra tính hợp lệ của query sau khi load
            if (entityQueries == null || !entityQueries.TryGetValue(queryKey, out var query) || string.IsNullOrWhiteSpace(query))
            {
                throw new KeyNotFoundException($"Không tìm thấy query '{queryKey}' cho thực thể '{entityName}'. " +
                    $"Vui lòng kiểm tra lại file {entityName}Queries.json trong thư mục Queries.");
            }

            return query;
        }

        /// <summary>
        /// Tải nội dung từ tệp JSON vào bộ nhớ đệm.
        /// </summary>
        /// <param name="entityName">Tên thực thể để xác định tên file.</param>
        /// <exception cref="FileNotFoundException">
        /// Ném ra khi không tìm thấy file {EntityName}Queries.json trong thư mục thực thi.
        /// </exception>
        private static void LoadQueries(string entityName)
        {
            // Trong môi trường ASP.NET Core, BaseDirectory thường trỏ vào thư mục bin.
            // Hãy đảm bảo các file JSON được set "Copy to Output Directory: Copy if newer"
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Queries", $"{entityName}Queries.json");
            
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                _queries[entityName] = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new();
            }
            else
            {
                throw new FileNotFoundException($"Không tìm thấy file cấu hình query tại đường dẫn: {filePath}. " +
                    "Hãy chắc chắn bạn đã chuột phải vào file JSON và chọn 'Copy if newer'.");
            }
        }
    }
}