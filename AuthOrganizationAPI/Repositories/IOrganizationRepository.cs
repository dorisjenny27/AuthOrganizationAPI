using AuthOrganizationAPI.Models.Entities;

namespace AuthOrganizationAPI.Repositories
{
    public interface IOrganizationRepository
    {
        Task<IEnumerable<Organization>> GetUserOrganizationsAsync(string userId);
        Task<IEnumerable<Organization>> GetOwnedOrganizationsAsync(string userId);
        Task<Organization> GetOrganizationByIdAsync(string orgId, string userId);
        Task<Organization> CreateOrganizationAsync(Organization organization);
        Task AddUserToOrganizationAsync(string orgId, string userId);
    }
}
