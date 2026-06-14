using System;
using System.Collections.Generic;
using System.Linq;
using Core.DTO;
using Core.Entities.Enums;

namespace Core.Validators
{
    /// <summary>
    /// Bộ quy tắc kiểm tra dữ liệu cho Thành phần lương (Không sử dụng thư viện ngoài)
    /// </summary>
    /// Created by Gemini Code Assist 10/06/2026
    public class SalaryCompositionAppValidator : BaseAppValidator<SalaryCompositionDTO>
    {
        /// <summary>
        /// Thực hiện kiểm tra tính hợp lệ của DTO
        /// </summary>
        /// <param name="dto">Dữ liệu thành phần lương</param>
        /// <returns>Từ điển chứa lỗi (Key: Tên trường, Value: Thông báo lỗi)</returns>
        public override Dictionary<string, string> Validate(SalaryCompositionDTO dto)
        {
            var errors = new Dictionary<string, string>();

            if (dto == null)
            {
                errors.Add("General", "Dữ liệu không được để trống.");
                return errors;
            }

            // 1. Mã thành phần: Bắt buộc, không quá 255 ký tự
            RuleForRequired(errors, nameof(dto.ScCode), dto.ScCode, "Mã thành phần lương không được để trống.");
            RuleForMaxLength(errors, nameof(dto.ScCode), dto.ScCode, 255, "Mã thành phần lương không vượt quá 255 ký tự.");

            // 2. Tên thành phần: Bắt buộc, không quá 255 ký tự
            RuleForRequired(errors, nameof(dto.ScName), dto.ScName, "Tên thành phần lương không được để trống.");
            RuleForMaxLength(errors, nameof(dto.ScName), dto.ScName, 255, "Tên thành phần lương không vượt quá 255 ký tự.");

            // 3. Loại thành phần: Kiểm tra tính hợp lệ trong Enum
            RuleForEnum(errors, nameof(dto.ScType), typeof(SalaryCompositionType), dto.ScType, "Loại thành phần lương không hợp lệ.");

            // 4. Tính chất: Kiểm tra tính hợp lệ trong Enum
            RuleForEnum(errors, nameof(dto.ScNature), typeof(SalaryCompositionNature), dto.ScNature, "Tính chất thành phần lương không hợp lệ.");

            // 5. Logic: Miễn thuế một phần (PartiallyExempt) 
            // Yêu cầu nhập công thức phần chịu thuế/miễn thuế
            if (dto.TaxStatus == TaxStatus.PartiallyExempt && string.IsNullOrWhiteSpace(dto.TaxableExpression))
            {
                errors.Add(nameof(dto.TaxableExpression), "Vui lòng nhập công thức tính phần chịu thuế/miễn thuế.");
            }

            // 6. Đơn vị áp dụng: Bắt buộc chọn ít nhất 1 đơn vị
            if (dto.AppliedOrganizations == null || !dto.AppliedOrganizations.Any())
            {
                errors.Add(nameof(dto.AppliedOrganizations), "Vui lòng chọn ít nhất một đơn vị áp dụng.");
            }

            // 7. Thành phần cộng tổng: Bắt buộc nếu phương thức tính là tự động cộng tổng (AutoSum)
            if (dto.CalculationMethod == CalculationMethod.AutoSum && string.IsNullOrWhiteSpace(dto.CompositionCode))
            {
                errors.Add(nameof(dto.CompositionCode), "Vui lòng chọn thành phần lương để cộng giá trị.");
            }

            return errors;
        }
    }
}