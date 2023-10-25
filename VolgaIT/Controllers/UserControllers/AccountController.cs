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
        private DataBaseContext _context;

        public AccountController(IOptions<JWTSettings> optAccess, DataBaseContext context)
        {
            _options = optAccess.Value;
            _context = context;
        }


        [HttpGet("Me")]
        [Authorize]
        public ActionResult<UserEntity> Me()
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            long userId = HelperWithJWT.instance.UserId(headers);

            UserEntity userEntity = _context.Users.FirstOrDefault(u => u.Id == userId);
            
            if (userEntity == null)
                ModelState.AddModelError("User", "Ошибка при нахождении информации о вас");
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(Helper.ConvertTo<UserEntity, UserNoId>(userEntity, new UserEntity()));
        }

        // получение jwt-token
        [HttpPost("SignIn")]
        public ActionResult<string> SignIn(UnicUser unicUser)
        {
            UserEntity user = null;
            user = _context.Users.FirstOrDefault(u => u.Username == unicUser.Username && u.Password == unicUser.Password);

            if (user == null)
                ModelState.AddModelError("User", "Такого пользователя не существует!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimValueTypes.Integer64, (user.Id).ToString()));
            if(user.IsAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            else
                claims.Add(new Claim(ClaimTypes.Role, "User"));

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
        public ActionResult SignUp(UnicUser user)
        {
            if (_context.Users.FirstOrDefault(u => u.Username == user.Username) != null)
                ModelState.AddModelError("Username", "Нельзя создать пользователя с существующим username!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Users.Add(new UserEntity() { Username = user.Username, Password = user.Password});
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost("SignOut")]
        [Authorize]
        public ActionResult<string> SignOut()
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            HelperWithJWT.instance.LogoutToken(headers);

            return Ok("Sign out success");
        }

        [HttpPut("Update")]
        [Authorize]
        public ActionResult<string> Update(UnicUser unicUser)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            long userId = HelperWithJWT.instance.UserId(headers);

            if (_context.Users.FirstOrDefault(u => u.Id != userId && u.Username == unicUser.Username) != null)
                ModelState.AddModelError("Username", "Нельзя использовать уже используемый в системе username!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            UserEntity user = _context.Users.FirstOrDefault(u => u.Id == userId);
            user.Username = unicUser.Username;
            user.Password = unicUser.Password;

            _context.Users.Update(user);
            _context.SaveChanges();

            return Ok("Ваши логин или/и пароль был(и) изменен(ы)!");
        }
    }
}
