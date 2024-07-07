using AuthOrganizationAPI.ExceptionHandler;
using AuthOrganizationAPI.Models.DTOs;
using AuthOrganizationAPI.Models.Entities;
using AuthOrganizationAPI.Repositories;
using AuthOrganizationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthOrganizationAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
       // private readonly IOrganizationRepository _orgRepository;
        private readonly IConfiguration _configuration;
        private readonly IOrganizationService _organizationService;

        public AuthService(UserManager<AppUser> userManager, IConfiguration configuration, IOrganizationService organizationService)
        {
            _userManager = userManager;
         //   _orgRepository = orgRepository;
            _configuration = configuration;
            _organizationService = organizationService;
        }

        public async Task<(string Token, ObjectResult Error)> RegisterUserAsync(AppUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (null, ErrorHandler.GetErrorResponse(errors, StatusCodes.Status400BadRequest));
            }
            var organizationName = $"{user.FirstName}'s Organisation";
            var org = new Organization
            {
                Name = organizationName,
                Description = $"Default organization for {user.FirstName} {user.LastName}",
                CreatedBy = user.Id
            };
            await _organizationService.CreateOrganizationAsync(org);
           // await _orgRepository.AddUserToOrganizationAsync(org.OrganizationId, user.Id);

            var token = GenerateJwtToken(user);
            return (token, null);
        }



        public async Task<(string Token, AppUser User, ObjectResult Error)> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                return (null, null, ErrorHandler.GetErrorResponse("Invalid email or password", StatusCodes.Status401Unauthorized));
            }

            var token = GenerateJwtToken(user);
            return (token, user, null);
        }


        private string GenerateJwtToken(AppUser user)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

            var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Issuer"],
            claims,
            expires: expires,
            signingCredentials: creds
        );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


}

