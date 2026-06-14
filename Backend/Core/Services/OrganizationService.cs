using Core.DTO;
using Core.Entities;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.Validators; // Import IAppValidator
using Core.Mappers;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services
{
    public class OrganizationService : CrudService<Organization, OrganizationDTO, IOrganizationRepository>, IOrganizationService
    {
        public OrganizationService(IOrganizationRepository repository, IAppValidator<OrganizationDTO> validator)
            : base(repository, validator)
        {
        }

        protected override OrganizationDTO MapToDto(Organization entity)
            => OrganizationMapper.ToResponseDTO(entity);

        protected override Organization MapToEntity(OrganizationDTO dto)
            => OrganizationMapper.ToEntity(dto);

        protected override void MapToEntity(Organization entity, OrganizationDTO dto)
            => OrganizationMapper.UpdateFromDto(entity, dto);

        /// <summary>
        /// Lấy danh sách đơn vị và xây dựng cấu trúc cây
        /// </summary>
        public async Task<List<OrganizationDTO>> GetOrganizationTreeAsync()
        {
            // 1. Lấy toàn bộ danh sách phẳng từ Repo
            var entities = await _repository.GetAllAsync();
            
            // 2. Map sang DTO
            var allNodes = entities.Select(MapToDto).ToList();
            
            // 3. Sử dụng Dictionary để truy xuất nhanh theo Id
            var nodeDict = allNodes.ToDictionary(n => n.OrganizationId);
            var rootNodes = new List<OrganizationDTO>();

            foreach (var node in allNodes)
            {
                if (node.OrganizationParentId == null || !nodeDict.TryGetValue(node.OrganizationParentId.Value, out OrganizationDTO? parent))
                {
                    rootNodes.Add(node);
                }
                else
                {
                    parent.Children.Add(node);
                }
            }
            return rootNodes;
        }
    }
}