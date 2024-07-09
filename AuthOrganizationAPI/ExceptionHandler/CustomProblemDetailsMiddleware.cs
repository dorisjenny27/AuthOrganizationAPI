//namespace AuthOrganizationAPI.ExceptionHandler
//{
//    public class CustomProblemDetailsMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public CustomProblemDetailsMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            context.Response.OnStarting(() =>
//            {
//                if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 600)
//                {
//                    context.Response.ContentType = "application/json";
//                    var originalBodyStream = context.Response.Body;
//                    var newBodyStream = new MemoryStream();
//                    context.Response.Body = newBodyStream;

//                    newBodyStream.Position = 0;
//                    var responseBody = new StreamReader(newBodyStream).ReadToEnd();
//                    newBodyStream.Position = 0;

//                    var modifiedResponseBody = responseBody.Replace("\"traceId\":", string.Empty);

//                    context.Response.Body = originalBodyStream;
//                    return context.Response.WriteAsync(modifiedResponseBody);
//                }
//                return Task.CompletedTask;
//            });

//            await _next(context);
//        }
//    }
//}
