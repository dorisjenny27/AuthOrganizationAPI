using AuthOrganizationAPI.Helpers;
using AuthOrganizationAPI.Models.DTOs;
using AuthOrganizationAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AuthOrganizationAPI.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<PagedResult<Organization>> GetUserOrganizationsAsync(string userId, int pageNumber, int pageSize);
        Task CreateOrganizationAsync(Organization organization);
        Task<Organization> GetOrganizationByIdAsync(string orgId, string userId);
        Task<CreateOrganizationResult> CreateOrganisationAsync(CreateOrganizationRequest request, string createdBy);
        Task<AddUserToOrganizationResponse> AddUserToOrganizationAsync(string orgId, AddUserToOrganizationRequest request, string userId);
    }
}
