using AuthOrganizationAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AuthOrganizationAPI.Models.DTOs
{
    public class LoginResult
    {
        public string Token { get; set; }
        public AppUser User { get; set; }
        public ObjectResult Error { get; set; }
    }
}
