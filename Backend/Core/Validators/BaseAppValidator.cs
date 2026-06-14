using System;
using System.Collections.Generic;
using Core.Interfaces.Validators;

namespace Core.Validators
{
    public abstract class BaseAppValidator<T> : IAppValidator<T>
    {
        public abstract Dictionary<string, string> Validate(T dto);

        public bool IsValid(T dto, out Dictionary<string, string> errors)
        {
            errors = Validate(dto);
            return errors.Count == 0;
        }

        // --- Các hàm tiện ích (Helpers) ---

        protected void RuleForRequired(Dictionary<string, string> errors, string propertyName, object value, string message)
        {
            if (value == null || (value is string s && string.IsNullOrWhiteSpace(s)))
            {
                AddError(errors, propertyName, message);
            }
        }

        protected void RuleForMaxLength(Dictionary<string, string> errors, string propertyName, string value, int maxLength, string message)
        {
            if (value?.Length > maxLength)
            {
                AddError(errors, propertyName, message);
            }
        }

        protected void RuleForEnum(Dictionary<string, string> errors, string propertyName, Type enumType, object value, string message)
        {
            if (!Enum.IsDefined(enumType, value))
            {
                AddError(errors, propertyName, message);
            }
        }

        protected void AddError(Dictionary<string, string> errors, string propertyName, string message)
        {
            errors.TryAdd(propertyName, message);
        }
    }
}