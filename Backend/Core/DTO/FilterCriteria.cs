namespace Core.DTO
{
    /// <summary>
    /// Các phép toán logic hỗ trợ trong việc lọc dữ liệu.
    /// </summary>
    /// Created by Phuong 26/02/2026
    public enum FilterOperation
    {
        /// <summary> Chứa chuỗi </summary>
        Contains,
        /// <summary> Không chứa chuỗi </summary>
        NotContains,
        /// <summary> Bắt đầu bằng </summary>
        StartsWith,
        /// <summary> Kết thúc bằng </summary>
        EndsWith,
        /// <summary> Bằng tuyệt đối </summary>
        Equals,
        /// <summary> Khác tuyệt đối </summary>
        NotEqual,
        /// <summary> Trống (null hoặc rỗng) </summary>
        Empty,
        /// <summary> Không trống </summary>
        NotEmpty
    }

    /// <summary>
    /// Đối tượng chứa thông tin tiêu chí lọc sau khi đã được phân tích.
    /// </summary>
    /// Created by Phuong 26/02/2026
    public class FilterCriteria
    {
        /// <summary>
        /// Tên trường/cột dữ liệu cần lọc (thường là PascalCase từ Frontend).
        /// </summary>
        public string Column { get; set; } = null!;

        /// <summary>
        /// Phép toán logic áp dụng.
        /// </summary>
        public FilterOperation Operation { get; set; }

        /// <summary>
        /// Giá trị dùng để so khớp.
        /// </summary>
        public string? Value { get; set; }

        public FilterCriteria() { }

        public FilterCriteria(string column, FilterOperation operation, string? value)
        {
            Column = column;
            Operation = operation;
            Value = value;
        }
    }
}
