using ProjectBase.Api.Concrete;
using ProjectBase.Business.Abstract;
using ProjectBase.Business.Concrete;
using ProjectBase.DTO.Auth;
using ProjectBase.Entity.Request.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ProjectBase.Entity.Enum.GlobalEnum;


namespace ProjectBase.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ApiController
    {

        private readonly AuthenticaionJWtService _authenticaionJWtService;
        private readonly IWebHostEnvironment _env;
        public AuthController(IAuthenticaionJwtService authenticaionJwtService, IWebHostEnvironment env)
            :base(authenticaionJwtService)
        {
            _authenticaionJWtService = (AuthenticaionJWtService?)authenticaionJwtService;
            _env = env;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loginResult = await _authenticaionJWtService.LoginAsync(new LoginRequestDTO
            {
                Username = request.Username,
                Password = request.Password
            });

            return JsonResponse(loginResult);

        }

        [HttpGet("RegisterDev")]
        public async Task<IActionResult> RegisterDevAsync()
        {
            if (!_env.IsDevelopment())
            {
                return BadRequest(new
                {
                    Message = "This endpoint is only available in the Development environment."
                });
            }
            RegistrationDTO dto = new RegistrationDTO
            {
                AcceptTerms = true,
                Email = "dev@dev.com",
                Name = "dev",
                Surname = "dev",
                Password = "Aa1234567*",
                PasswordRepeat = "Aa1234567*",
                Role = "dev",
                Username = "dev",
                UserType = UserType.SuperAdmin
            };

            var response = await _authenticaionJWtService.RegistrationAsync(dto);

            return Ok(response);

        }

    }
}
