using AuthOrganizationAPI.ExceptionHandler;
using AuthOrganizationAPI.Models.DTOs;
using AuthOrganizationAPI.Models.Entities;
using AuthOrganizationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
            // VAlidate the model
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model,serviceProvider: null, items: null);
            bool isValid = Validator.TryValidateObject(model, context, validationResults, true);

            if (string.IsNullOrEmpty(model.FirstName))
            {
                validationResults.Add(new ValidationResult("Firstname must not be null", new[] { "FirstName" }));
            }
            if (string.IsNullOrEmpty(model.LastName))
            {
                validationResults.Add(new ValidationResult("Lastname must not be null", new[] { "LastName" }));
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                validationResults.Add(new ValidationResult("Email must not be null", new[] { "Email" }));
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                validationResults.Add(new ValidationResult("Password must not be null", new[] { "Password" }));
            }
            if (string.IsNullOrEmpty(model.Phone))
            {
                validationResults.Add(new ValidationResult("Phone must not be null", new[] { "Phone" }));
            }
            if(!isValid|| validationResults.Count > 0)
            {
                var errors = validationResults.Select(result => new
                {
                    field = result.MemberNames.FirstOrDefault(),
                    message = result.ErrorMessage
                });
                return UnprocessableEntity(new {errors});
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
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        errors = new[] { new { field = "", message = registerUserResponse.Error.Value.ToString() } }
                    });
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
            // VAlidate the model
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            bool isValid = Validator.TryValidateObject(model, context, validationResults, true);

            if (string.IsNullOrEmpty(model.Email))
            {
                validationResults.Add(new ValidationResult("Email must not be null", new[] { "Email" }));
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                validationResults.Add(new ValidationResult("Password must not be null", new[] { "Password" }));
            }
            if (!isValid || validationResults.Count > 0)
            {
                var errors = validationResults.Select(result => new
                {
                    field = result.MemberNames.FirstOrDefault(),
                    message = result.ErrorMessage
                });
                return UnprocessableEntity(new { errors });
            }
                try
            {
                var loginResult = await _authService.LoginAsync(model.Email, model.Password);

                if (loginResult.Error != null)
                    return StatusCode(StatusCodes.Status401Unauthorized, new
                    {
                        errors = new[] { new { field = "", message = loginResult.Error.Value.ToString() } }
                    });

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