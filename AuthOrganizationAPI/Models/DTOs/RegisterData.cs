using AuthOrganizationAPI.Models.Entities;

namespace AuthOrganizationAPI.Models.DTOs
{
    public class RegisterData
    {
      public string AccessToken { get; set; }
      public RegisterUserDTO User { get; set; }
        
    }
}
