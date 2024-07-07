using Microsoft.AspNetCore.Mvc;

namespace AuthOrganizationAPI.ExceptionHandler
{
    public static class ErrorHandler
    {
        public static ObjectResult GetErrorResponse(string message, int statusCode)
        {
            string status;
            switch (statusCode)
            {
                case StatusCodes.Status400BadRequest:
                    status = "Bad request";
                    break;
                case StatusCodes.Status401Unauthorized:
                    status = "Unauthorized";
                    break;
                // Add more cases for other status codes as needed
                default:
                    status = "Error";
                    break;
            }

            string errorMessage;
            if (statusCode == StatusCodes.Status401Unauthorized && message == "Invalid email or password")
            {
                errorMessage = "Authentication failed";
            }
            else
            {
                errorMessage = message;
            }

            return new ObjectResult(new
            {
                status,
                message = errorMessage,
                statusCode
            })
            {
                StatusCode = statusCode
            };
        }
    }
}
