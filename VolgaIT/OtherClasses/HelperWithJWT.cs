using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace VolgaIT.OtherClasses
{
    public class HelperWithJWT
    {
        public static HelperWithJWT helper;
        public static HelperWithJWT instance { get
            {
                if(helper == null)
                    helper = new HelperWithJWT();

                return helper;
            } }

        private List<string> blackList = new List<string>();

        public long UserId(string headers)
        {
            string token = headers.Split(' ')[1];

            var helper = new JwtSecurityTokenHandler();
            var jwt = helper.ReadToken(token);

            var claim = (jwt as JwtSecurityToken).Claims.FirstOrDefault(c => c.Type == ClaimValueTypes.Integer64).Value;

            return long.Parse(claim);
        }

        public void LogoutToken(string headers)
        {
            this.blackList.Add(headers.Split(' ')[1]);
        }

        public bool TokenIsValid(string headers)
        {
            string token = headers.Split(' ')[1];

            if(blackList.FirstOrDefault(t => t == token) != null)
                return false;

            return true;
        }

    }
}
