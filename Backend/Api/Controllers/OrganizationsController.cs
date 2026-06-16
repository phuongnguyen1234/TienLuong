using Core.DTO;
using Core.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class OrganizationsController : BaseCrudController<OrganizationDTO>
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationsController(IOrganizationService organizationService) : base(organizationService)
        {
            _organizationService = organizationService;
        }

        /// <summary>
        /// Lấy danh sách đơn vị công tác theo cấu trúc cây
        /// </summary>
        [HttpGet("tree")]
        public async Task<ActionResult<Response>> GetOrganizationTree()
        {
            var tree = await _organizationService.GetOrganizationTreeAsync();
            return Success(tree);
        }
    }
}