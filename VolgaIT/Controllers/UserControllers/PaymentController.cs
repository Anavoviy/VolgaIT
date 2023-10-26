using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolgaIT.EntityDB;
using VolgaIT.Model.Entities;
using VolgaIT.OtherClasses;

namespace VolgaIT.Controllers.UserControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private DataBaseContext _context; 

        public PaymentController(DataBaseContext context)
        {
            _context = context;
        }


        [HttpPost("Hesoyam/{accountId}")]
        [Authorize]
        public ActionResult Hesoyam(long accountId)
        {
            if (accountId == 0 || _context.Users.FirstOrDefault(u => u.Id == accountId) == null)
                return BadRequest("Пользователя с таким идентификатором не существует!");


            string headers = this.HttpContext.Request.Headers.Authorization.ToString();

            UserEntity user = new UserEntity();
            
            if(!HelperWithJWT.instance.UserIsAdmin(headers))
                user = _context.Users.FirstOrDefault(u => u.Id == HelperWithJWT.instance.UserId(headers));
            else
                user = _context.Users.FirstOrDefault(u => u.Id == accountId);

            user.Balance += 250000;    

            _context.Users.Update(user);
            _context.SaveChanges();

            return Ok();
        }

    }
}
