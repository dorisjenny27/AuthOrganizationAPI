using AuthOrganizationAPI.ExceptionHandler;
using AuthOrganizationAPI.Models.DTOs;
using AuthOrganizationAPI.Models.Entities;
using AuthOrganizationAPI.Services;
using AuthOrganizationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthOrganizationAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _orgService;

        public OrganizationController(IOrganizationService orgService)
        {
            _orgService = orgService;
        }

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
        public async Task<IActionResult> CreateOrganisation([FromBody] CreateOrganizationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Status = "Bad Request",
                    Message = "Client error",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new
                {
                    status = "Bad Request",
                    message = "User not authenticated",
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

         //   request.CreatedBy = userId;


            var result = await _orgService.CreateOrganisationAsync(request, userId);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    Status = "Bad Request",
                    Message = result.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }


            return StatusCode(StatusCodes.Status201Created, new
            {
                Status = "success",
                Message = result.Message,
                Data = new
                {
                    OrgId = result.Organization.OrganizationId,
                    Name = result.Organization.Name,
                    Description = result.Organization.Description,
                }
            });
        }


        [AllowAnonymous]
        [HttpPost("{orgId}/users")]
        public async Task<IActionResult> AddUserToOrganization(string orgId, [FromBody] AddUserToOrganizationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = "Bad Request",
                    message = "Invalid request data",
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var response = await _orgService.AddUserToOrganizationAsync(orgId, request, userId);

            if (response.Message != "User added to organisation successfully")
            {
                return BadRequest(new
                {
                    status = "Bad Request",
                    message = response.Message,
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            return Ok(new
            {
                status = "success",
                message = response.Message
            });
        }

    }
}
