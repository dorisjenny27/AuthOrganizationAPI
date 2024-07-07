using AuthOrganizationAPI.ExceptionHandler;
using AuthOrganizationAPI.Helpers;
using AuthOrganizationAPI.Models.DTOs;
using AuthOrganizationAPI.Models.Entities;
using AuthOrganizationAPI.Repositories;
using AuthOrganizationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthOrganizationAPI.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _orgRepository;
        private readonly UserManager<AppUser> _userManager;

        public OrganizationService(IOrganizationRepository orgRepository, UserManager<AppUser> userManager)
        {
            _orgRepository = orgRepository;
            _userManager = userManager;
        }


        public async Task<PagedResult<Organization>> GetUserOrganizationsAsync(string userId, int pageNumber, int pageSize)
        {
            var userOrganizations = await _orgRepository.GetUserOrganizationsAsync(userId);
            var ownedOrganizations = await _orgRepository.GetOwnedOrganizationsAsync(userId);
            var organizations = userOrganizations.Concat(ownedOrganizations).Distinct();
            return Utilities.GetPaged(organizations, pageNumber, pageSize);
        }


        public async Task CreateOrganizationAsync(Organization organization)
        {
            await _orgRepository.CreateOrganizationAsync(organization);
        }


        public async Task<Organization> GetOrganizationByIdAsync(string orgId, string userId)
        {
            var organization = await _orgRepository.GetOrganizationByIdAsync(orgId, userId);
            return organization;
        }


        public async Task<CreateOrganizationResult> CreateOrganisationAsync(CreateOrganizationRequest request, string createdBy)
        {
            if (string.IsNullOrEmpty(createdBy))
            {
                return new CreateOrganizationResult
                {
                    Success = false,
                    Message = "User ID is required"
                };
            }

            var organization = new Organization
            {
                Name = request.Name,
                Description = request.Description,
                CreatedBy = createdBy,
            };

            try
            {
                var createdOrg = await _orgRepository.CreateOrganizationAsync(organization);
                return new CreateOrganizationResult
                {
                    Success = true,
                    Message = "Organisation created successfully",
                    Organization = createdOrg
                };
            }
            catch (Exception ex)
            {
                return new CreateOrganizationResult
                {
                    Success = false,
                    Message = "An error occurred while creating the organization"
                };
            }
        }


        public async Task<AddUserToOrganizationResponse> AddUserToOrganizationAsync(string orgId, AddUserToOrganizationRequest request, string userId)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return new AddUserToOrganizationResponse { Message = "User ID is required" };
            }

            var organization = await _orgRepository.GetOrganizationByIdAsync(orgId, userId);
            if (organization == null)
            {
                return new AddUserToOrganizationResponse { Message = "Organization not found or you don't have access" };
            }

            await _orgRepository.AddUserToOrganizationAsync(orgId, request.UserId);

            return new AddUserToOrganizationResponse { Message = "User added to organisation successfully" };
        }

    }
}

