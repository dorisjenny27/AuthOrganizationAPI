using AuthOrganizationAPI.Helpers;
using AuthOrganizationAPI.Models.DTOs;
using AuthOrganizationAPI.Models.Entities;
using AuthOrganizationAPI.Repositories;
using AuthOrganizationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

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




        public async Task<Organization> CreateOrganizationAsync(Organization org, string userId)
        {
            var createdOrg = await _orgRepository.CreateAsync(org);
          //  await _orgRepository.AddUserToOrganizationAsync(createdOrg.OrgId, userId);
            return createdOrg;
        }

        public async Task AddUserToOrganizationAsync(string orgId, string userId)
        {
            await _orgRepository.AddUserToOrganizationAsync(orgId, userId);
        }
    }
}
