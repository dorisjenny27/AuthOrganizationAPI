using AuthOrganizationAPI.Models.DTOs;
using AuthOrganizationAPI.Models.Entities;
using AuthOrganizationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthOrganizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _orgService;

        public OrganizationController(IOrganizationService orgService)
        {
            _orgService = orgService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserOrganizations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var pagedOrganizations = await _orgService.GetUserOrganizationsAsync(userId, pageNumber, pageSize);

            var response = new
            {
                status = "success",
                message = $"Retrieved {pagedOrganizations.Data.Count()} organizations out of {pagedOrganizations.TotalCount}",
                data = new
                {
                    organizations = pagedOrganizations.Data.Select(org => new
                    {
                        orgId = org.OrganizationId,
                        name = org.Name,
                        description = org.Description
                    }),
                    pagination = new
                    {
                        currentPage = pagedOrganizations.CurrentPage,
                        totalPages = pagedOrganizations.TotalPages
                    }
                }
            };

            return Ok(response);
        }


        [Authorize]
        [HttpGet("{orgId}")]
        public async Task<IActionResult> GetOrganizationById(string orgId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var organization = await _orgService.GetOrganizationByIdAsync(orgId, userId);

            if (organization == null)
                return NotFound();

            var response = new
            {
                status = "success",
                message = $"Retrieved organization with ID {orgId}",
                data = new
                {
                    orgId = organization.OrganizationId.ToString(),
                    name = organization.Name,
                    description = organization.Description
                }
            };

            return Ok(response);
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrganization([FromBody] CreateOrgModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var org = new Organization { Name = model.Name, Description = model.Description };
            var createdOrg = await _orgService.CreateOrganizationAsync(org, userId);
            return CreatedAtAction(nameof(GetUserOrganizations), new { id = createdOrg.OrganizationId }, createdOrg);
        }

        [HttpPost("{orgId}/users")]
        public async Task<IActionResult> AddUserToOrganization(string orgId, [FromBody] AddUserModel model)
        {
            await _orgService.AddUserToOrganizationAsync(orgId, model.UserId);
            return Ok();
        }
    }
}
