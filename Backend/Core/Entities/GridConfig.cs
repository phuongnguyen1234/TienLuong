namespace Core.Entities
{
    /// <summary>
    /// Thực thể tương ứng với bảng pa_grid_config
    /// </summary>
    public class GridConfig
    {
        /// <summary>
        /// Khóa chính là GridKey (ví dụ: pa_salary_composition)
        /// </summary>
        public string GridKey { get; set; } = string.Empty;
        public string ConfigData { get; set; } = string.Empty;
    }
}