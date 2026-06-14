using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    /// <summary>
    /// Lớp object phân trang
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// Created by Phuong 26/02/2026
    public class PagedResult<T>
    {
        /// <summary>
        /// Dữ liệu
        /// </summary>
        public IEnumerable<T> Items { get; }

        /// <summary>
        /// Số trang
        /// </summary>
        public int Page { get; }

        /// <summary>
        /// Kích thước trang
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public long TotalItems { get; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages => PageSize == 0 ? 0 : (int)((TotalItems + PageSize - 1) / PageSize);

        public PagedResult(IEnumerable<T> items, int page, int pageSize, long totalItems)
        {
            Items = items ?? Array.Empty<T>();
            Page = page < 1 ? 1 : page;
            PageSize = pageSize < 1 ? 10 : pageSize;
            TotalItems = totalItems;
        }
    }
}
