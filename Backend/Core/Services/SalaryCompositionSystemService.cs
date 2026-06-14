using Core.DTO;
using Core.Entities;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.Validators; // Import IAppValidator
using Core.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services
{
    public class SalaryCompositionSystemService : CrudService<SalaryCompositionSystem, SalaryCompositionSystemDTO, ISalaryCompositionSystemRepository>, ISalaryCompositionSystemService
    {
        public SalaryCompositionSystemService(ISalaryCompositionSystemRepository repository, IAppValidator<SalaryCompositionSystemDTO> validator)
            : base(repository, validator)
        {
        }

        protected override SalaryCompositionSystemDTO MapToDto(SalaryCompositionSystem entity)
            => SalaryCompositionSystemMapper.ToResponseDTO(entity);

        protected override SalaryCompositionSystem MapToEntity(SalaryCompositionSystemDTO dto)
            => SalaryCompositionSystemMapper.ToEntity(dto);

        protected override void MapToEntity(SalaryCompositionSystem entity, SalaryCompositionSystemDTO dto)
            => SalaryCompositionSystemMapper.UpdateFromDto(entity, dto);

        public async Task<PagedResult<SalaryCompositionSystemDTO>> GetPagingAsync(string? searchTerm, List<string>? filters, int pageIndex, int pageSize)
        {
            var criteria = FilterQueryParser.Parse(filters);

            var (entities, totalCount) = await _repository.GetPagingAsync(searchTerm, criteria, pageIndex, pageSize);
            
            var dtos = entities.Select(MapToDto).ToList();
            
            return new PagedResult<SalaryCompositionSystemDTO>(
                dtos, 
                pageIndex, 
                pageSize, 
                totalCount);
        }

        public async Task<int> BulkCloneAsync(List<Guid> systemIds)
        {
            if (systemIds == null || systemIds.Count == 0) return 0;
            
            var result = await _repository.BulkCloneAsync(systemIds);
            return result;
        }
    }
}