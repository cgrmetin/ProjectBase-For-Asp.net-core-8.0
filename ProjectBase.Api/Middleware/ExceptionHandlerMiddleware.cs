using ProjectBase.Entity.Global;
using ProjectBase.Entity.Response;

namespace ProjectBase.Api.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var errorResponse = new ApiError
            {
                ErrorCode = 5000,
                ErrorMessage = "An unexpected error occurred",
                Detail = exception.Message
            };

            if (exception is KeyNotFoundException)
            {
                errorResponse.ErrorCode = 4040;
                errorResponse.ErrorMessage = "Resource not found";
            }
            else if (exception is UnauthorizedAccessException)
            {
                errorResponse.ErrorCode = 4030;
                errorResponse.ErrorMessage = "Access denied";
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;

            context.Items[CommonConstantGenerator.ApiConstant.LastError] = errorResponse;

            return Task.CompletedTask;
        }
    }
}
