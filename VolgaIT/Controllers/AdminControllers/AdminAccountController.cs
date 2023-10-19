using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolgaIT.EntityDB;
using VolgaIT.Model.Model;
using VolgaIT.OtherClasses;

namespace VolgaIT.Controllers.AdminControllers
{
    [Route("api/Admin/Account")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminAccountController : ControllerBase
    {
        private DataContext _context;

        public AdminAccountController(DataContext context) => _context = context;


        [HttpGet]
        public ActionResult<List<User>> GetUsers()
        {
            if(_context.Users == null)
                return BadRequest(null);
            else
                return Ok(_context.Users.ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(long id)
        {
            User user = CEAM.UserEntityToModel(_context.Users.SingleOrDefault(u => u.Id == id));
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
            {
                _context.Users.Add(CEAM.UserModelToEntity(user));
                return Ok();
            }
        }

        [HttpPut("{id}")]
        public ActionResult EditUser(long id, User user) 
        { 
            if(user.Username == "")
                ModelState.AddModelError("Username", "Нельзя создать пользователя с уже существующим username!");

            if (!ModelState.IsValid)
                return BadRequest();

            else
            {
                _context.Users.Update(CEAM.UserModelToEntity(user));
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(long id)
        {
            if (id == 0)
                ModelState.AddModelError("Id", "Пользователя с таким идентификатором не существует в системе!");

            if(!ModelState.IsValid)
                return BadRequest();

            _context.Users.Remove(_context.Users.FirstOrDefault(u => u.Id == id));

            return Ok();
        }



    }
}
