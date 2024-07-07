using AuthOrganizationAPI.Models.Entities;

namespace AuthOrganizationAPI.Models.DTOs
{
    public class UserOrganization
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
