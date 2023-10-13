using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolgaIT.Model;

namespace VolgaIT.Controllers.AdminControllers
{
    [Route("api/Admin/Account")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminAccountController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<User>> GetUsers()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(long id)
        {

            return Ok();
        }

        [HttpPost]
        public ActionResult AddUser(User user)
        {
            if (user.Username == "")
                ModelState.AddModelError("Username", "Нельзя создать пользователя с уже существующим username!");
            
            if (!ModelState.IsValid)
                return BadRequest();
        
            else 
                return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult EditUser(long id, User user) 
        { 
            if(user.Username == "")
                ModelState.AddModelError("Username", "Нельзя создать пользователя с уже существующим username!");

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(long id)
        {
            if (id == 0)
                ModelState.AddModelError("Id", "Пользователя с таким идентификатором не существует в системе!");

            if(!ModelState.IsValid)
                return BadRequest();

            return Ok();
        }



    }
}
