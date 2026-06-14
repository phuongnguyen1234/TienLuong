﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    /// <summary>
    /// Lớp cung cấp các phương thức mở rộng cho kiểu dữ liệu string.
    /// </summary>
    /// Created by Phuong 25/02/2026
    public static class StringExtension
    {
        /// <summary>
        /// Chuyển đổi chuỗi sang định dạng snake_case.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSnakeCase(this string input)
        {
            return Regex
                .Replace(input, @"([a-z0-9])([A-Z])", "$1_$2")
                .ToLower();
        }

        /// <summary>
        /// Chuyển đổi chuỗi từ snake_case sang PascalCase.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return Regex.Replace(input, @"(?:^|_)([a-z0-9])", match => 
                match.Groups[1].Value.ToUpper());
        }
    }
}
