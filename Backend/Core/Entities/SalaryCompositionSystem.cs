using System;

namespace Core.Entities
{
    /// <summary>
    /// Thực thể Thành phần lương hệ thống
    /// </summary>
    /// Created by Phuong 25/05/2026
    public class SalaryCompositionSystem : BaseSalaryComposition
    {
        public Guid ScsId { get; set; }

        /// <summary>
        /// Đánh dấu thành phần lương hệ thống đã được người dùng chọn đưa vào sử dụng hay chưa
        /// </summary>
        public bool ScsIsInUsed { get; set; } // tinyint (0/1) -> bool

        // Ghi chú: Các thuộc tính chung (Code, Name, Type, Nature...) được kế thừa từ BaseSalaryComposition.
        // Việc ánh xạ prefix "scs_" từ Database sẽ được xử lý tại tầng Repository thông qua CustomPropertyTypeMap của Dapper.
    }
}