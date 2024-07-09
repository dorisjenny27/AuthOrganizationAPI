using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthOrganizationAPI.ExceptionHandler
{
    public class CustomValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = new List<object>();

                foreach (var key in context.ModelState.Keys)
                {
                    var fieldErrors = context.ModelState[key].Errors
                        .Select(error => new { field = key, message = error.ErrorMessage })
                        .ToList();
                    errors.AddRange(fieldErrors);
                }

                context.Result = new JsonResult(new { errors })
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
