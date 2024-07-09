using AuthOrganizationAPI.Controllers;
using AuthOrganizationAPI.Models.DTOs;
using AuthOrganizationAPI.Models.Entities;
using AuthOrganizationAPI.Services;
using AuthOrganizationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AuthOrganizationAPI.Tests.tests
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_ShouldCreateUserWithDefaultOrganization()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "john@example.com",
                Password = "StrongPassword123!",
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890"
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(x => x.RegisterUserAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(new RegisterUserResponse { Token = "fake_token" });

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerModel) as ObjectResult;

            // Assert
            Assert.NotNull(result); // Ensure the result is not null
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);

            var responseBody = result.Value as RegisterResponseModel;
            Assert.NotNull(responseBody); // Ensure the response body is not null

            // Assert response values
            Assert.Equal("success", responseBody.Status);
            Assert.Equal("Registration successful", responseBody.Message);
            Assert.Equal("fake_token", responseBody.Data.AccessToken);
            Assert.Equal("john@example.com", responseBody.Data.User.Email);
            Assert.Equal("John", responseBody.Data.User.FirstName);
            Assert.Equal("Doe", responseBody.Data.User.LastName);
            Assert.Equal("1234567890", responseBody.Data.User.Phone);
        }



        [Fact]
        public async Task Login_ShouldSucceedWithValidCredentials()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "john@example.com",
                Password = "StrongPassword123!"
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new LoginResult
                {
                    Token = "fake_token",
                    User = new AppUser
                    {
                        Id = "user_id",
                        Email = "john@example.com",
                        FirstName = "John",
                        LastName = "Doe"
                    }
                });

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Login(loginModel) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

            var responseBody = result.Value as RegisterResponseModel;
            Assert.Equal("success", responseBody.Status);
            Assert.Equal("Login successful", responseBody.Message);
            Assert.Equal("fake_token", responseBody.Data.AccessToken);
            Assert.Equal("john@example.com", responseBody.Data.User.Email);
        }


        [Theory]
        [InlineData("", "Doe", "john@example.com", "StrongPassword123!", "First Name is required")]
        [InlineData("John", "", "john@example.com", "StrongPassword123!", "Last Name is required")]
        [InlineData("John", "Doe", "", "StrongPassword123!", "Email is required")]
        [InlineData("John", "Doe", "john@example.com", "", "Password is required")]
        public async Task Register_ShouldFailWithMissingRequiredFields(string firstName, string lastName, string email, string password, string expectedErrorMessage)
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password
            };

            var mockAuthService = new Mock<IAuthService>();
            var controller = new AuthController(mockAuthService.Object);

            // Simulate model
            // error
            if (string.IsNullOrEmpty(firstName))
                controller.ModelState.AddModelError(nameof(RegisterModel.FirstName), "First Name is required");
            if (string.IsNullOrEmpty(lastName))
                controller.ModelState.AddModelError(nameof(RegisterModel.LastName), "Last Name is required");
            if (string.IsNullOrEmpty(email))
                controller.ModelState.AddModelError(nameof(RegisterModel.Email), "Email is required");
            if (string.IsNullOrEmpty(password))
                controller.ModelState.AddModelError(nameof(RegisterModel.Password), "Password is required");

            // Act
            var result = await controller.Register(registerModel) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);

            var responseBody = result.Value as RegisterResponseModel;
            Assert.Contains(expectedErrorMessage, responseBody.Message);
        }


        [Fact]
        public async Task Register_ShouldFailWithDuplicateEmail()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "existing@example.com",
                Password = "StrongPassword123!",
                FirstName = "John",
                LastName = "Doe"
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(x => x.RegisterUserAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(new RegisterUserResponse
                {
                    Error = new ObjectResult(new RegisterResponseModel
                    {
                        Status = "error",
                        Message = "Email already exists"
                    })
                    {
                        StatusCode = StatusCodes.Status422UnprocessableEntity
                    }
                });

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerModel) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);

            var responseBody = result.Value as RegisterResponseModel;
            Assert.Equal("error", responseBody.Status);
            Assert.Equal("Email already exists", responseBody.Message);
        }


        [Fact]
        public async Task Register_ShouldCreateDefaultOrganisation()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "john@example.com",
                Password = "StrongPassword123!",
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890"
            };

            var mockAuthService = new Mock<IAuthService>();
            var mockOrgService = new Mock<IOrganizationService>();
            var mockUserManager = UserManagerMockHelper.MockUserManager(new List<AppUser>());

            mockAuthService.Setup(x => x.RegisterUserAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(new RegisterUserResponse { Token = "fake_token" });

            mockOrgService.Setup(x => x.CreateOrganisationAsync(It.IsAny<CreateOrganizationRequest>(), It.IsAny<string>()))
                .ReturnsAsync(new CreateOrganizationResult { Success = true, Organization = new Organization { Name = "John's Organization" } });

            var authService = new AuthService(
                mockUserManager.Object,
                new Mock<IConfiguration>().Object,
                mockOrgService.Object);

            var controller = new AuthController(authService);

            // Act
            var result = await controller.Register(registerModel) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);

            mockOrgService.Verify(x => x.CreateOrganisationAsync(
                It.Is<CreateOrganizationRequest>(r => r.Name == "John's Organization"),
                It.IsAny<string>()), Times.Once);
        }





        [Fact]
        public async Task Login_ShouldFailWithInvalidCredentials()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "john@example.com",
                Password = "WrongPassword"
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new LoginResult
                {
                    Error = new UnauthorizedObjectResult(new { message = "Invalid email or password" })
                });

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Login(loginModel) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);

            var responseBody = result.Value as dynamic;
            Assert.Equal("Invalid email or password", responseBody.message);
        }

        [Fact]
        public async Task Register_ShouldFailWithDuplicateUserId()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "john@example.com",
                Password = "StrongPassword123!",
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890"
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(x => x.RegisterUserAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(new RegisterUserResponse
                {
                    Error = new ObjectResult(new RegisterResponseModel
                    {
                        Status = "error",
                        Message = "User ID already exists"
                    })
                    {
                        StatusCode = StatusCodes.Status422UnprocessableEntity
                    }
                });

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerModel) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);

            var responseBody = result.Value as RegisterResponseModel;
            Assert.Equal("error", responseBody.Status);
            Assert.Equal("User ID already exists", responseBody.Message);
        }

    }
}