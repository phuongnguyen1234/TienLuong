using Core.DTO;
using Core.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            return Ok(new Response(true, null, 200) { Data = tree });
        }
    }
}