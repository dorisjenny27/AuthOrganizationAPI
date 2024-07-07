using System.ComponentModel.DataAnnotations;

namespace AuthOrganizationAPI.Models.DTOs
{
    public class AddUserToOrganizationRequest
    {
        [Required]
        public string UserId { get; set; }
    }
}
