using Core.DTO;
using Core.Entities;

namespace Core.Mappers
{
    /// <summary>
    /// Mapper cho SalaryCompositionSystem
    /// </summary>
    public static class SalaryCompositionSystemMapper
    {
        public static SalaryCompositionSystem ToEntity(SalaryCompositionSystemDTO dto)
        {
            return new SalaryCompositionSystem
            {
                ScsId = dto.ScsId,
                ScCode = dto.ScsCode,
                ScName = dto.ScsName,
                ScType = dto.ScsType,
                ScNature = dto.ScsNature,
                ScTaxStatus = dto.ScsTaxStatus,
                ScIsTaxDeductible = dto.ScsIsTaxDeductible,
                ScTaxableExpression = dto.ScsTaxableExpression,
                ScExemptExpression = dto.ScsExemptExpression,
                ScLimitExpression = dto.ScsLimitExpression,
                ScAllowExceedLimit = dto.ScsAllowExceedLimit,
                ScValueType = dto.ScsValueType,
                ScCalculationMethod = dto.ScsCalculationMethod,
                ScAggregationScope = dto.ScsAggregationScope,
                ScCompositionCode = dto.ScsCompositionCode,
                ScOrganizationLevel = dto.ScsOrganizationLevel,
                ScFormulaExpression = dto.ScsFormulaExpression,
                ScDescription = dto.ScsDescription ?? string.Empty,
                ScIsDisplayedOnPayroll = dto.ScsIsDisplayedOnPayroll,
                ScsIsInUsed = dto.ScsIsInUsed,
                ScIsDeleted = false
            };
        }

        public static void UpdateFromDto(SalaryCompositionSystem entity, SalaryCompositionSystemDTO dto)
        {
            entity.ScName = dto.ScsName;
            entity.ScType = dto.ScsType;
            entity.ScNature = dto.ScsNature;
            entity.ScTaxStatus = dto.ScsTaxStatus;
            entity.ScIsTaxDeductible = dto.ScsIsTaxDeductible;
            entity.ScTaxableExpression = dto.ScsTaxableExpression;
            entity.ScExemptExpression = dto.ScsExemptExpression;
            entity.ScLimitExpression = dto.ScsLimitExpression;
            entity.ScValueType = dto.ScsValueType;
            entity.ScCalculationMethod = dto.ScsCalculationMethod;
            entity.ScAggregationScope = dto.ScsAggregationScope;
            entity.ScCompositionCode = dto.ScsCompositionCode;
            entity.ScOrganizationLevel = dto.ScsOrganizationLevel;
            entity.ScFormulaExpression = dto.ScsFormulaExpression;
            entity.ScDescription = dto.ScsDescription ?? string.Empty;
            entity.ScIsDisplayedOnPayroll = dto.ScsIsDisplayedOnPayroll;
            entity.ScsIsInUsed = dto.ScsIsInUsed;
            entity.ScAllowExceedLimit = dto.ScsAllowExceedLimit;
        }

        public static SalaryCompositionSystemDTO ToResponseDTO(SalaryCompositionSystem entity)
        {
            return new SalaryCompositionSystemDTO
            {
                ScsId = entity.ScsId,
                ScsCode = entity.ScCode,
                ScsName = entity.ScName,
                ScsType = entity.ScType,
                ScsNature = entity.ScNature,
                ScsTaxStatus = entity.ScTaxStatus,
                ScsIsTaxDeductible = entity.ScIsTaxDeductible,
                ScsTaxableExpression = entity.ScTaxableExpression,
                ScsExemptExpression = entity.ScExemptExpression,
                ScsLimitExpression = entity.ScLimitExpression,
                ScsValueType = entity.ScValueType,
                ScsCalculationMethod = entity.ScCalculationMethod,
                ScsAggregationScope = entity.ScAggregationScope,
                ScsCompositionCode = entity.ScCompositionCode,
                ScsOrganizationLevel = entity.ScOrganizationLevel,
                ScsFormulaExpression = entity.ScFormulaExpression,
                ScsDescription = entity.ScDescription,
                ScsIsDisplayedOnPayroll = entity.ScIsDisplayedOnPayroll,
                ScsIsInUsed = entity.ScsIsInUsed,
                ScsAllowExceedLimit = entity.ScAllowExceedLimit
            };
        }
    }
}