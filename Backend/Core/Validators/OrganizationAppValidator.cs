using System;
using System.Collections.Generic;
using Core.DTO;

namespace Core.Validators
{
    /// <summary>
    /// Bộ quy tắc kiểm tra dữ liệu cho Đơn vị
    /// </summary>
    public class OrganizationAppValidator : BaseAppValidator<OrganizationDTO>
    {
        public override Dictionary<string, string> Validate(OrganizationDTO dto)
        {
            var errors = new Dictionary<string, string>();

            if (dto == null)
            {
                errors.Add("General", "Dữ liệu không được để trống.");
                return errors;
            }

            RuleForRequired(errors, nameof(dto.OrganizationName), dto.OrganizationName, "Tên đơn vị không được để trống.");
            RuleForMaxLength(errors, nameof(dto.OrganizationName), dto.OrganizationName, 255, "Tên đơn vị không quá 255 ký tự.");

            // Đơn vị cha không được trùng với đơn vị hiện tại
            if (dto.OrganizationParentId.HasValue && dto.OrganizationId != Guid.Empty && dto.OrganizationParentId.Value == dto.OrganizationId)
            {
                AddError(errors, nameof(dto.OrganizationParentId), "Đơn vị cha không được trùng với đơn vị hiện tại.");
            }

            return errors;
        }
    }
}