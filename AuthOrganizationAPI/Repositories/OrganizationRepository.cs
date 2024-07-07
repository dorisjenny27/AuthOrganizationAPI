using AuthOrganizationAPI.Data;
using AuthOrganizationAPI.Models.DTOs;
using AuthOrganizationAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthOrganizationAPI.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly AppDbContext _context;

        public OrganizationRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<Organization> GetOrganizationByIdAsync(string orgId, string userId)
        {
            var organization = await _context.Organizations
                .Where(o => o.OrganizationId.ToString() == orgId && o.CreatedBy == userId)
                .FirstOrDefaultAsync();

            return organization;
        }

        //public async Task<Organization> GetByIdAsync(string orgId)
        //{
        //    return await _context.Organizations.FindAsync(orgId);
        //}

        public async Task<IEnumerable<Organization>> GetUserOrganizationsAsync(string userId)
        {
            var userOrganizations = await _context.UserOrganizations
                .Where(uo => uo.UserId == userId)
                .Select(uo => uo.Organization)
                .ToListAsync();

            return userOrganizations;
        }


        public async Task<IEnumerable<Organization>> GetOwnedOrganizationsAsync(string userId)
        {
            var ownedOrganizations = await _context.Organizations
                .Where(o => o.CreatedBy == userId)
                .ToListAsync();

                return ownedOrganizations;
        }


        public async Task CreateOrganizationAsync(Organization organization)
        {
            await _context.Organizations.AddAsync(organization);
            await _context.SaveChangesAsync();
        }


        public async Task<Organization> CreateAsync(Organization org)
        {
            await _context.Organizations.AddAsync(org);
            await _context.SaveChangesAsync();
            return org;
        }

        public async Task AddUserToOrganizationAsync(string orgId, string userId)
        {
           // var userOrg = new Organization { Id = userId, OrgId = orgId };
         //   await _context.Organizations.AddAsync(userOrg);
            await _context.SaveChangesAsync();
        }
        
    }
}
