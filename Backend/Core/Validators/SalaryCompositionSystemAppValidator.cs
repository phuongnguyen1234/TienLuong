using System;
using System.Collections.Generic;
using Core.DTO;

namespace Core.Validators
{
    /// <summary>
    /// Bộ quy tắc kiểm tra dữ liệu cho Thành phần lương hệ thống
    /// </summary>
    public class SalaryCompositionSystemAppValidator : BaseAppValidator<SalaryCompositionSystemDTO>
    {
        public override Dictionary<string, string> Validate(SalaryCompositionSystemDTO dto)
        {
            var errors = new Dictionary<string, string>();

            if (dto == null)
            {
                errors.Add("General", "Dữ liệu không được để trống.");
                return errors;
            }

            RuleForRequired(errors, nameof(dto.ScsCode), dto.ScsCode, "Mã thành phần lương không được để trống.");
            RuleForMaxLength(errors, nameof(dto.ScsCode), dto.ScsCode, 255, "Mã không quá 255 ký tự.");

            RuleForRequired(errors, nameof(dto.ScsName), dto.ScsName, "Tên thành phần lương không được để trống.");
            RuleForMaxLength(errors, nameof(dto.ScsName), dto.ScsName, 255, "Tên không quá 255 ký tự.");

            return errors;
        }
    }
}