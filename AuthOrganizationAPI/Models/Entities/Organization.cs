using AuthOrganizationAPI.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthOrganizationAPI.Models.Entities
{
    public class Organization
    {
        [Key]
        public string OrganizationId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public ICollection<UserOrganization> UserOrganizations { get; set; }
    }
}
