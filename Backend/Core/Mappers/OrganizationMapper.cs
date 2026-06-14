using Core.DTO;
using Core.Entities;

namespace Core.Mappers
{
    /// <summary>
    /// Mapper chuyển đổi cho Organization
    /// </summary>
    public static class OrganizationMapper
    {
        public static Organization ToEntity(OrganizationDTO dto)
        {
            return new Organization
            {
                OrganizationId = dto.OrganizationId,
                OrganizationName = dto.OrganizationName,
                OrganizationParentId = dto.OrganizationParentId
            };
        }

        public static void UpdateFromDto(Organization entity, OrganizationDTO dto)
        {
            entity.OrganizationName = dto.OrganizationName;
            entity.OrganizationParentId = dto.OrganizationParentId;
        }

        public static OrganizationDTO ToResponseDTO(Organization entity)
        {
            return new OrganizationDTO
            {
                OrganizationId = entity.OrganizationId,
                OrganizationName = entity.OrganizationName,
                OrganizationParentId = entity.OrganizationParentId
            };
        }
    }
}