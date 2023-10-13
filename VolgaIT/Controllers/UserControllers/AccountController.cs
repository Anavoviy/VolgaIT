using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VolgaIT.Model;

namespace VolgaIT.Controllers.UserControllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTSettings _options;

        public AccountController(IOptions<JWTSettings> optAccess)
        {
            _options = optAccess.Value;
        }


        [HttpGet("Me")]
        [Authorize]
        public ActionResult Me()
        {
            return Ok();
        }

        // получение jwt-token
        [HttpPost("SignIn")]
        public ActionResult<string> SignIn(User user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Username));
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

            var jwt = new JwtSecurityToken(
                            issuer: _options.Issuer,
                            audience: _options.Audience,
                            claims: claims,
                            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)),
                            notBefore: DateTime.UtcNow,
                            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        [HttpPost("SignUp")]
        public ActionResult<string> SignUp(User user)
        {
            if (user.Username == "")
                ModelState.AddModelError("Username", "Нельзя создать пользователя с существующим username!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok();
        }

        [HttpPost("SignOut")]
        [Authorize]
        public ActionResult<string> SignOut()
        {
            return Ok("Sign out success");
        }

        [HttpPut("Update")]
        [Authorize]
        public ActionResult<string> Update(User user)
        {
            if (user.Username == "")
                ModelState.AddModelError("Username", "Нельзя использовать уже используемые в системе username!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok();
        }
    }
}
