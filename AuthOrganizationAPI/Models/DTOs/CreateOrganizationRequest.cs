using System.ComponentModel.DataAnnotations;

namespace AuthOrganizationAPI.Models.DTOs
{
    public class CreateOrganizationRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
