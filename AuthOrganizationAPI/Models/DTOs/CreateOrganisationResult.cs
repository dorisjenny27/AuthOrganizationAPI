using AuthOrganizationAPI.Models.Entities;

namespace AuthOrganizationAPI.Models.DTOs
{
    public class CreateOrganisationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Organization Organisation { get; set; }
    }
}
