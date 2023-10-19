using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VolgaIT.EntityDB;
using VolgaIT.Model.ModelUniqueDataTransfers;
using VolgaIT.Model.DTO;
using VolgaIT.Model.Entities;
using VolgaIT.Model.Model;
using VolgaIT.OtherClasses;
using JWT;

namespace VolgaIT.Controllers.UserControllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTSettings _options;
        private HttpContext _httpContext;
        private DataContext _context;

        public AccountController(IOptions<JWTSettings> optAccess, DataContext context)
        {
            _options = optAccess.Value;
            _context = context;
            _context.Database.EnsureCreated();
        }


        [HttpGet("Me")]
        [Authorize]
        public ActionResult<UserEntity> Me()
        {
            return Ok();
        }

        // получение jwt-token
        [HttpPost("SignIn")]
        public ActionResult<string> SignIn(UnicUser unicUser)
        {
            try
            {
                User user = CEAM.UserEntityToModel(_context.Users.FirstOrDefault(u => u.Username == unicUser.Username && u.Password == unicUser.Password));

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimValueTypes.Integer64, (102).ToString()));
                claims.Add(new Claim(ClaimTypes.Name, unicUser.Username));
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
            } catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost("SignUp")]
        public ActionResult SignUp(UnicUser user)
        {
            //if (_context.Users.FirstOrDefault(u => u.Username == user.Username) != null)
            //    ModelState.AddModelError("Username", "Нельзя создать пользователя с существующим username!");

            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            _context.Users.Add(CEAM.UserModelToEntity(new User() { Username = user.Username, Password = user.Password }));
            _context.SaveChanges();

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
        public ActionResult<string> Update(UnicUser dtoUser)
        {
            if (_context.Users.FirstOrDefault(u => u.Username == dtoUser.Username) != null)
                ModelState.AddModelError("Username", "Нельзя использовать уже используемый в системе username!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string headers =  this.HttpContext.Request.Headers.Authorization.ToString();
            long userId = HelperWithJWT.UserId(headers);

            UserEntity user = _context.Users.FirstOrDefault(u => u.Id == userId);
            user.Username = dtoUser.Username;
            user.Password = dtoUser.Password;

            _context.Users.Update(user);
            _context.SaveChanges();

            return Ok(userId.ToString());
        }
    }
}
