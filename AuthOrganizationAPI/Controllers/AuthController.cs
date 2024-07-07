using AuthOrganizationAPI.ExceptionHandler;
using AuthOrganizationAPI.Models.DTOs;
using AuthOrganizationAPI.Models.Entities;
using AuthOrganizationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthOrganizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.Phone,
                };
                var (token, error) = await _authService.RegisterUserAsync(user, model.Password);
                if (error != null)
                    return error;

                return StatusCode(StatusCodes.Status201Created, new
                {
                    status = "success",
                    message = "Registration successful",
                    data = new
                    {
                        accessToken = token,
                        user = new
                        {
                            userId = user.Id,
                            firstName = user.FirstName,
                            lastName = user.LastName,
                            email = user.Email,
                            phone = user.PhoneNumber
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return ErrorHandler.GetErrorResponse(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var (token, user, error) = await _authService.LoginAsync(model.Email, model.Password);

                if (error != null)
                    return error;

                return Ok(new
                {
                    status = "success",
                    message = "Login successful",
                    data = new
                    {
                        accessToken = token,
                        user = new
                        {
                            userId = user.Id,
                            firstName = user.FirstName,
                            lastName = user.LastName,
                            email = user.Email,
                            phone = user.PhoneNumber
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return ErrorHandler.GetErrorResponse(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }
    }
}