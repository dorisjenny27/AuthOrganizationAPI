using System.ComponentModel.DataAnnotations;

namespace AuthOrganizationAPI.Models.DTOs
{
    public class CreateOrganizationRequest
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
    }
}
