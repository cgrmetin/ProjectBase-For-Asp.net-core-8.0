using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ProjectBase.DTO.Auth;
using ProjectBase.Entity.Database;

namespace ProjectBase.Business.Abstract
{
    public interface IAuthenticaionJwtService
    {
        Task<Status> LoginAsync(LoginRequestDTO model);
        Task<Status> RegistrationAsync(RegistrationDTO model);
        Task LogoutAsync();
        Task<Status> ChangePasswordAsync(ChangePasswordDTO model, AppUser user);
        Task<AppUser> GetCurrentUser();
        JwtSecurityToken GenerateJwtToken(IEnumerable<Claim> authClaims = null);
    }
}
