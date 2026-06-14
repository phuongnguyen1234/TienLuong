namespace Core.DTO
{
    /// <summary>
    /// Phép lọc dữ liệu
    /// </summary>
    /// Created by Phuong 26/02/2026
    public enum FilterOperation
    {
        Contains,
        NotContains,
        StartsWith,
        EndsWith,
        Equals,
        NotEqual,
        Empty,
        NotEmpty
    }

    /// <summary>
    /// Tiêu chí lọc dữ liệu
    /// </summary>
    /// Created by Phuong 26/02/2026
    public class FilterCriteria
    {
        /// <summary>
        /// Tên cột
        /// </summary>
        public string Column { get; set; } = null!;

        /// <summary>
        /// Phép lọc
        /// </summary>
        public FilterOperation Operation { get; set; }

        /// <summary>
        /// Giá trị lọc
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
