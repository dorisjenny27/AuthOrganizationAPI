using AuthOrganizationAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AuthOrganizationAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(string Token, ObjectResult Error)> RegisterUserAsync(AppUser user, string password);
        Task<(string Token, AppUser User, ObjectResult Error)> LoginAsync(string email, string password);
    }
}
