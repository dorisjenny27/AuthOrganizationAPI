using AuthOrganizationAPI.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthOrganizationAPI.Models.Entities
{
    public class AppUser : IdentityUser
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        public ICollection<UserOrganization> UserOrganizations { get; set; }

    }
}
