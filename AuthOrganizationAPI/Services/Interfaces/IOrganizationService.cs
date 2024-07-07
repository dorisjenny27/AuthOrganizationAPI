using AuthOrganizationAPI.Helpers;
using AuthOrganizationAPI.Models.Entities;

namespace AuthOrganizationAPI.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<PagedResult<Organization>> GetUserOrganizationsAsync(string userId, int pageNumber, int pageSize);
        Task CreateOrganizationAsync(Organization organization);
        Task<Organization> GetOrganizationByIdAsync(string orgId, string userId);











        Task<Organization> CreateOrganizationAsync(Organization org, string userId);
        Task AddUserToOrganizationAsync(string orgId, string userId);
    }
}
