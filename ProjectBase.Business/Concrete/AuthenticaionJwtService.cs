using ProjectBase.Business.Abstract;
using ProjectBase.DTO.Auth;
using ProjectBase.Entity.Attributes;
using ProjectBase.Entity.Database;
using ProjectBase.Entity.Global;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectBase.Business.Concrete
{
    [IocContainerItem(typeof(IAuthenticaionJwtService))]
    public class AuthenticaionJWtService : IAuthenticaionJwtService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationService _localizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticaionJWtService(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager, IConfiguration configuration, 
            ILocalizationService localizationService,IHttpContextAccessor httpContextAccessor)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
            _localizationService = localizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Status> ChangePasswordAsync(ChangePasswordDTO model, AppUser user)
        {
            var status = new Status();

            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = "Kullanıcı Bulunamadı";
                return status;
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = string.Join(", ", result.Errors);
                return status;
            }

            status.StatusCode = 1;
            status.Message = "Şifre Başarıyla Değiştirildi";
            return status;
        }

        public Task<AppUser> GetCurrentUser()
        {
            throw new NotImplementedException();
        }

        public async Task<Status> LoginAsync(LoginRequestDTO model)
        {
            var status = new Status();
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = await _localizationService.GetStringAsync(CommonConstantGenerator.ReourceCode.ErrorMessage,
                    CommonConstantGenerator.ResourceKey.WrongUserName);
                return status;
            }

            if (!user.IsActive)
            {
                status.StatusCode = 0;
                status.Message = await _localizationService.GetStringAsync(CommonConstantGenerator.ReourceCode.ErrorMessage,
                    CommonConstantGenerator.ResourceKey.PassiveUser);
                return status;
            }

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                status.StatusCode = 0;
                status.Message = await _localizationService.GetStringAsync(CommonConstantGenerator.ReourceCode.ErrorMessage,
                    CommonConstantGenerator.ResourceKey.WrongPassword);
                return status;
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
                {
                    new Claim("Language", _httpContextAccessor.HttpContext?.Request.Headers["Accept-Language"].ToString().Split('-').FirstOrDefault() ?? "en"),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }; 
            
            foreach (var claim in authClaims)
            {
                Debug.WriteLine($"Claim: {claim.Type} = {claim.Value}");
            }

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GenerateJwtToken(authClaims);

            status.StatusCode = 1;
            status.Message = await _localizationService.GetStringAsync(CommonConstantGenerator.ReourceCode.SuccessMessage,
                CommonConstantGenerator.ResourceKey.LoginSuccess);
            status.Token = new JwtSecurityTokenHandler().WriteToken(token);
            status.Expiration = token.ValidTo;

            return status;
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Status> RegistrationAsync(RegistrationDTO model)
        {
            var status = new Status();

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.Message = await _localizationService.GetStringAsync(CommonConstantGenerator.ReourceCode.ErrorMessage,
                    CommonConstantGenerator.ResourceKey.EmailAddressAlreadyRegistered);
                return status;
            }

            var user = new AppUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = model.Name,
                LastName = model.Surname,
                Email = model.Email,
                UserName = model.Username,
                EmailConfirmed = true,
                UserType = model.UserType,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = JsonConvert.SerializeObject(result.Errors);
                return status;
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new AppRole { Name = model.Role});
            }

            await _userManager.AddToRoleAsync(user, model.Role);

            status.StatusCode = 1;
            status.Message = await _localizationService.GetStringAsync(CommonConstantGenerator.ReourceCode.SuccessMessage,
                CommonConstantGenerator.ResourceKey.RegisteredSuccesfuly);
            return status;
        }

        public JwtSecurityToken GenerateJwtToken(IEnumerable<Claim> authClaims = null)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            // Eğer kullanıcı login olmuşsa, ClaimsPrincipal'dan Claims al
            if (authClaims == null && user.Identity?.IsAuthenticated == true)
            {
                authClaims = user.Claims;
            }

            if (authClaims == null || !authClaims.Any())
            {
                throw new InvalidOperationException("Claims cannot be null or empty. Please provide valid claims.");
            }

            // JWT oluşturma işlemleri
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JWT:TokenLifetimeMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}
