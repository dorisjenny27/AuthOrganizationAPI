using AuthOrganizationAPI.Models.Entities;

namespace AuthOrganizationAPI.Repositories
{
    public interface IOrganizationRepository
    {
       // Task<Organization> GetByIdAsync(string orgId);
        Task<IEnumerable<Organization>> GetUserOrganizationsAsync(string userId);
        Task<IEnumerable<Organization>> GetOwnedOrganizationsAsync(string userId);
        Task CreateOrganizationAsync(Organization organization);
        Task<Organization> GetOrganizationByIdAsync(string orgId, string userId);


        Task<Organization> CreateAsync(Organization org);
        Task AddUserToOrganizationAsync(string orgId, string userId);

    }
}
