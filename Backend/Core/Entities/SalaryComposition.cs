using System;
using System.Collections.Generic;
using Core.Entities.Enums;

namespace Core.Entities
{
    /// <summary>
    /// Thực thể tương ứng với bảng pa_salary_composition
    /// </summary>
    public class SalaryComposition : BaseSalaryComposition
    {
        public Guid ScId { get; set; }
        
        /// <summary>
        /// 0: Copy từ hệ thống, 1: Tự thêm
        /// </summary>
        public SalaryCompositionSource ScSource { get; set; }
        
        /// <summary>
        /// 0: Đang theo dõi, 1: Ngừng theo dõi
        /// </summary>
        public SalaryStatus ScStatus { get; set; }

        // Các thuộc tính khác (sc_code, sc_name, sc_type...) kế thừa từ BaseSalaryComposition
        // và sẽ được Dapper map thông qua naming convention snake_case -> PascalCase
    }
}