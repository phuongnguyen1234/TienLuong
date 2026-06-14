using System;
using System.Linq;
using Core.DTO;
using Core.Entities;
using Core.Entities.Enums;

namespace Core.Mappers
{
    /// <summary>
    /// Chuyển đổi giữa SalaryComposition Entity và DTO
    /// </summary>
    public static class SalaryCompositionMapper
    {
        /// <summary>
        /// Chuyển từ Entity sang DTO để trả về Frontend
        /// </summary>
        public static SalaryCompositionDTO ToDto(SalaryComposition entity)
        {
            if (entity == null) return null;

            return new SalaryCompositionDTO
            {
                ScId = entity.ScId,
                ScCode = entity.ScCode,
                ScName = entity.ScName,
                ScType = entity.ScType,
                ScNature = entity.ScNature,
                TaxStatus = entity.ScTaxStatus,
                IsTaxDeductible = entity.ScIsTaxDeductible,
                TaxableExpression = entity.ScTaxableExpression,
                ExemptExpression = entity.ScExemptExpression,
                LimitExpression = entity.ScLimitExpression,
                AllowExceedLimit = entity.ScAllowExceedLimit,
                ValueType = entity.ScValueType,
                CalculationMethod = entity.ScCalculationMethod,
                AggregationScope = entity.ScAggregationScope,
                CompositionCode = entity.ScCompositionCode,
                OrganizationLevel = entity.ScOrganizationLevel,
                FormulaExpression = entity.ScFormulaExpression,
                Description = entity.ScDescription,
                IsDisplayedOnPayroll = entity.ScIsDisplayedOnPayroll,
                ScSource = entity.ScSource,
                ScStatus = entity.ScStatus
            };
        }

        /// <summary>
        /// Chuyển từ DTO (Input từ Form) sang Entity để lưu DB
        /// </summary>
        public static SalaryComposition ToEntity(SalaryCompositionDTO dto)
        {
            if (dto == null) return null;

            return new SalaryComposition
            {
                ScId = dto.ScId ?? Guid.NewGuid(),
                ScCode = dto.ScCode?.Trim(),
                ScName = dto.ScName?.Trim(),
                ScType = dto.ScType,
                ScNature = dto.ScNature,
                ScTaxStatus = dto.TaxStatus,
                ScIsTaxDeductible = dto.IsTaxDeductible,
                ScTaxableExpression = dto.TaxableExpression,
                ScExemptExpression = dto.ExemptExpression,
                ScLimitExpression = dto.LimitExpression,
                ScAllowExceedLimit = dto.AllowExceedLimit,
                ScValueType = dto.ValueType,
                ScCalculationMethod = dto.CalculationMethod,
                ScAggregationScope = dto.AggregationScope,
                ScCompositionCode = dto.CompositionCode,
                ScOrganizationLevel = dto.OrganizationLevel,
                ScFormulaExpression = dto.FormulaExpression,
                ScDescription = dto.Description,
                ScIsDisplayedOnPayroll = dto.IsDisplayedOnPayroll,
                ScSource = dto.ScSource,
                ScStatus = dto.ScStatus,
                ScIsDeleted = false
            };
        }

        /// <summary>
        /// Cập nhật dữ liệu từ DTO vào Entity hiện tại
        /// </summary>
        public static void UpdateFromDto(SalaryComposition entity, SalaryCompositionDTO dto)
        {
            if (entity == null || dto == null) return;

            entity.ScCode = dto.ScCode?.Trim();
            entity.ScName = dto.ScName?.Trim();
            entity.ScType = dto.ScType;
            entity.ScNature = dto.ScNature;
            entity.ScTaxStatus = dto.TaxStatus;
            entity.ScIsTaxDeductible = dto.IsTaxDeductible;
            entity.ScTaxableExpression = dto.TaxableExpression;
            entity.ScExemptExpression = dto.ExemptExpression;
            entity.ScLimitExpression = dto.LimitExpression;
            entity.ScAllowExceedLimit = dto.AllowExceedLimit;
            entity.ScValueType = dto.ValueType;
            entity.ScCalculationMethod = dto.CalculationMethod;
            entity.ScAggregationScope = dto.AggregationScope;
            entity.ScCompositionCode = dto.CompositionCode;
            entity.ScOrganizationLevel = dto.OrganizationLevel;
            entity.ScFormulaExpression = dto.FormulaExpression;
            entity.ScDescription = dto.Description;
            entity.ScIsDisplayedOnPayroll = dto.IsDisplayedOnPayroll;
            entity.ScStatus = dto.ScStatus;
        }
    }
}