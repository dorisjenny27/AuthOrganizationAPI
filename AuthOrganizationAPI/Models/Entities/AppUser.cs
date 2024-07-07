using AuthOrganizationAPI.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthOrganizationAPI.Models.Entities
{
    public class AppUser : IdentityUser
    {
        [Key]

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<UserOrganization> UserOrganizations { get; set; }

    }
}
