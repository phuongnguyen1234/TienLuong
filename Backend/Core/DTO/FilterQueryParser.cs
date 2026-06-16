﻿namespace Core.DTO
{
    /// <summary>
    /// Lớp tiện ích thực hiện chuyển đổi danh sách các chuỗi lọc thô từ Client thành đối tượng FilterCriteria.
    /// </summary>
    /// Created by Phuong 26/02/2026
    public static class FilterQueryParser
    {
        /// <summary>
        /// Phân tích mảng các chuỗi lọc có định dạng "Column:Operation:Value".
        /// </summary>
        /// <param name="rawFilters">Danh sách chuỗi lọc nhận được từ URL/Request.</param>
        /// <returns>Danh sách các đối tượng FilterCriteria đã được định danh toán tử.</returns>
        /// <example>
        /// Input: ["ScName:contains:Phuong", "ScStatus:eq:0"]
        /// Output: List với 2 Criteria tương ứng.
        /// </example>
        public static List<FilterCriteria>? Parse(IEnumerable<string>? rawFilters)
        {
            if (rawFilters == null) return null;

            var parsed = new List<FilterCriteria>();
            foreach (var raw in rawFilters)
            {
                if (string.IsNullOrWhiteSpace(raw)) continue;

                // Tách chuỗi làm 3 phần: [0] Cột, [1] Toán tử, [2] Giá trị
                // Dùng limit 3 để cho phép giá trị (value) chứa dấu hai chấm nếu cần
                var parts = raw.Split([':'], 3);
                if (parts.Length < 3) continue;

                var col = parts[0].Trim();
                if (string.IsNullOrEmpty(col)) continue;

                var opText = parts[1].Trim().ToLowerInvariant();
                var val = parts[2];

                // Ánh xạ chuỗi toán tử sang Enum tương ứng, hỗ trợ nhiều alias khác nhau
                var op = opText switch
                {
                    "contains" => FilterOperation.Contains,
                    "notcontains" => FilterOperation.NotContains,
                    "startswith" => FilterOperation.StartsWith,
                    "endswith" => FilterOperation.EndsWith,
                    "equals" => FilterOperation.Equals,
                    "eq" => FilterOperation.Equals, // Hỗ trợ viết tắt kiểu OData/Query String
                    "notequal" => FilterOperation.NotEqual,
                    "ne" => FilterOperation.NotEqual,
                    "empty" => FilterOperation.Empty,
                    "null" => FilterOperation.Empty, // Xử lý trường hợp Client gửi từ khóa null
                    "notempty" => FilterOperation.NotEmpty,
                    "notnull" => FilterOperation.NotEmpty,
                    _ => FilterOperation.Contains // Mặc định là tìm kiếm chứa chuỗi nếu không khớp
                };

                parsed.Add(new FilterCriteria(col, op, val));
            }

            return parsed.Count != 0 ? parsed : null;
        }
    }
}
