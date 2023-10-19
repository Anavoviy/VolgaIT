using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace VolgaIT.OtherClasses
{
    public class HelperWithJWT
    {
        public static long UserId(string headers)
        {
            string token = headers.Split(' ')[1];

            var helper = new JwtSecurityTokenHandler();
            var jwt = helper.ReadToken(token);

            var claim = (jwt as JwtSecurityToken).Claims.FirstOrDefault(c => c.Type == ClaimValueTypes.Integer64).Value;

            return long.Parse(claim);
        }

    }
}
