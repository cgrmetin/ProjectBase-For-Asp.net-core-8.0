using ProjectBase.Business.Abstract;
using ProjectBase.Entity.Response;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace ProjectBase.Api.Concrete
{
    public abstract class ApiController : ControllerBase
    {
        private readonly IAuthenticaionJwtService _authenticaionJWtService;
        protected ApiController(IAuthenticaionJwtService authenticaionJWtService)
        {
            _authenticaionJWtService = authenticaionJWtService;
        }

        protected IActionResult JsonResponse<T> (T model)
        {
            var controllerName = HttpContext.GetRouteData()?.Values["controller"]?.ToString();
            var actionName = HttpContext.GetRouteData()?.Values["action"]?.ToString();
                      
            GenericResponse response = new();

            if (HttpContext.Items.TryGetValue("LastError", out var error) && error is Exception exception)
            {
                response.IsError = true;
                response.ApiError = new ApiError
                {
                    ErrorCode = 5000,
                    ErrorMessage = exception.Message,
                    Detail = exception.StackTrace
                };

                return StatusCode(500, response);
            }

            response.IsError = false;
            response.ApiError = null;
            response.Response = model;

            int statusCode = (HttpContext.Request.Method == "POST") ? 201 : 200;

            if (controllerName == "Auth" && actionName == "Login")
            {
                return StatusCode(statusCode, response);
            }

            var jwt = _authenticaionJWtService.GenerateJwtToken();
            if (!string.IsNullOrEmpty(new JwtSecurityTokenHandler().WriteToken(jwt)))
            {
                Response.Headers.Add("Authorization", $"{new JwtSecurityTokenHandler().WriteToken(jwt)}");
            }

            return StatusCode(statusCode,response);
        }
    }
}
