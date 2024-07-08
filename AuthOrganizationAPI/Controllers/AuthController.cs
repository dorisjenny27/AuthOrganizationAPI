using AuthOrganizationAPI.ExceptionHandler;
using AuthOrganizationAPI.Models.DTOs;
using AuthOrganizationAPI.Models.Entities;
using AuthOrganizationAPI.Services;
using AuthOrganizationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthOrganizationAPI.Controllers
{
    [Route("[controller]")]
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
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault();
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new RegisterResponseModel
                {
                    Status = "error",
                    Message = errorMessage
                });
            }

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

                var registerUserResponse = await _authService.RegisterUserAsync(user, model.Password);

                if (registerUserResponse.Error != null)
                {
                    if (registerUserResponse.Error is ObjectResult badRequestResult)
                    {
                        return StatusCode(badRequestResult.StatusCode.Value, badRequestResult.Value);
                    }
                }

                return StatusCode(StatusCodes.Status201Created, new RegisterResponseModel
                {
                    Status = "success",
                    Message = "Registration successful",
                    Data = new RegisterData
                    {
                        AccessToken = registerUserResponse.Token,
                        User = new RegisterUserDTO
                        {
                            UserId = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            Phone = user.PhoneNumber
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Exception in Register method: {ex.Message}");

                // Return a generic server error response
                return StatusCode(StatusCodes.Status500InternalServerError, new RegisterResponseModel
                {
                    Status = "error",
                    Message = "An error occurred while processing your request."
                });
            }
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var loginResult = await _authService.LoginAsync(model.Email, model.Password);

                if (loginResult.Error != null)
                    return loginResult.Error;

                return Ok(new RegisterResponseModel
                {
                    Status = "success",
                    Message = "Login successful",
                    Data = new RegisterData
                    {
                        AccessToken = loginResult.Token,
                        User = new RegisterUserDTO
                        {
                            UserId = loginResult.User.Id,
                            FirstName = loginResult.User.FirstName,
                            LastName = loginResult.User?.LastName,
                            Email = loginResult.User?.Email,
                            Phone = loginResult.User.PhoneNumber
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