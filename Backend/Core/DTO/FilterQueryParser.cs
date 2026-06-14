﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    /// <summary>
    /// Phân tích query lọc
    /// </summary>
    public static class FilterQueryParser
    {
        // chuỗi lọc có dạng: "Column:operation:value"
        public static List<FilterCriteria>? Parse(IEnumerable<string>? rawFilters)
        {
            if (rawFilters == null) return null;

            var parsed = new List<FilterCriteria>();
            foreach (var raw in rawFilters)
            {
                if (string.IsNullOrWhiteSpace(raw)) continue;

                var parts = raw.Split(new[] { ':' }, 3);
                if (parts.Length < 3) continue;

                var col = parts[0].Trim();
                if (string.IsNullOrEmpty(col)) continue;

                var opText = parts[1].Trim().ToLowerInvariant();
                var val = parts[2];

                var op = opText switch
                {
                    "contains" => FilterOperation.Contains,
                    "notcontains" => FilterOperation.NotContains,
                    "startswith" => FilterOperation.StartsWith,
                    "endswith" => FilterOperation.EndsWith,
                    "equals" => FilterOperation.Equals,
                    "eq" => FilterOperation.Equals,
                    "notequal" => FilterOperation.NotEqual,
                    "ne" => FilterOperation.NotEqual,
                    "empty" => FilterOperation.Empty,
                    "null" => FilterOperation.Empty,
                    "notempty" => FilterOperation.NotEmpty,
                    "notnull" => FilterOperation.NotEmpty,
                    _ => FilterOperation.Contains
                };

                parsed.Add(new FilterCriteria(col, op, val));
            }

            return parsed.Any() ? parsed : null;
        }
    }
}
