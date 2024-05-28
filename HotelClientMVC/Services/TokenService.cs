using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt; 
using System.Linq;

namespace HotelClientMVC.Services
{
    public class TokenService
    {
        public bool IsUserAdmin(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var roleClaims = jsonToken?.Claims.Where(claim => claim.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            return roleClaims != null && roleClaims.Contains("Admin");
        }
    }
}
