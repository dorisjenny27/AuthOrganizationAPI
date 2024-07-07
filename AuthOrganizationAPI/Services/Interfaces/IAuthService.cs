using AuthOrganizationAPI.Models.DTOs;
using AuthOrganizationAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AuthOrganizationAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterUserResponse> RegisterUserAsync(AppUser user, string password);
        Task<LoginResult> LoginAsync(string email, string password);
    }
}
