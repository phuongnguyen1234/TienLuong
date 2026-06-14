using Core.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.Service
{
    public interface IOrganizationService : IBaseCrudService<OrganizationDTO>
    {
        Task<List<OrganizationDTO>> GetOrganizationTreeAsync();
    }
}