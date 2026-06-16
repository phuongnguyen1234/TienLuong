namespace Core.DTO
{
    public class GridConfigDTO
    {
        /// <summary>
        /// Khóa của Grid (ví dụ: pa_salary_composition)
        /// </summary>
        public string GridKey { get; set; } = string.Empty;
        public string ConfigData { get; set; } = string.Empty;
    }
}