using AuthOrganizationAPI.Models.Entities;

namespace AuthOrganizationAPI.Models.DTOs
{
    public class CreateOrganizationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Organization Organization { get; set; }
    }
}
