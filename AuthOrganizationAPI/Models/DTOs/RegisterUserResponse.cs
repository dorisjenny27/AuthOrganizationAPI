using AuthOrganizationAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AuthOrganizationAPI.Models.DTOs
{
    public class RegisterUserResponse
    {
        public string Token { get; set; }
        public ObjectResult Error { get; set; }
    }
}
