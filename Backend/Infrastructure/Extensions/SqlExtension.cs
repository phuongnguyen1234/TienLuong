using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Infrastructure.Extensions
{
    /// <summary>
    /// Extension để load SQL queries từ file JSON
    /// </summary>
    /// Created by Phuong 24/05/2026
    public static class SqlExtension
    {
        private static readonly Dictionary<string, Dictionary<string, string>> _queries = new();

        public static string GetQuery(string entityName, string queryKey)
        {
            if (!_queries.TryGetValue(entityName, out var entityQueries))
            {
                LoadQueries(entityName);
                _queries.TryGetValue(entityName, out entityQueries);
            }

            if (entityQueries == null || !entityQueries.TryGetValue(queryKey, out var query) || string.IsNullOrWhiteSpace(query))
            {
                throw new KeyNotFoundException($"Không tìm thấy query '{queryKey}' cho thực thể '{entityName}'. " +
                    $"Vui lòng kiểm tra lại file {entityName}Queries.json trong thư mục Queries.");
            }

            return query;
        }

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