using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolgaIT.EntityDB;
using VolgaIT.Model.Entities;
using VolgaIT.Model.Model;
using VolgaIT.OtherClasses;

namespace VolgaIT.Controllers.AdminControllers
{
    [Route("api/Admin/Account")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminAccountController : ControllerBase
    {
        private DataBaseContext _context;

        public AdminAccountController(DataBaseContext context) => _context = context;


        [HttpGet]
        public ActionResult<List<User>> GetUsers(int start = 0, int count = 0)
        {
            if(_context.Users == null)
                return BadRequest("Пользователей нет в базе данных!");

            List<UserEntity> usersEntity = new List<UserEntity>();

            if (count > 0)
                usersEntity = _context.Users.Skip(start).Take(count).OrderBy(u => u.Id).ToList();
            else
                usersEntity = _context.Users.Skip(start).ToList();

            List<User> users = new List<User>();
            foreach(UserEntity userEntity in usersEntity)
                users.Add(Helper.ConvertTo<UserEntity, User>(userEntity, new UserEntity()));

            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(long id)
        {
            if (id == 0)
                return BadRequest("Идентификатор не может быть равен нулю (0)!");

            User user = Helper.ConvertTo<UserEntity, User>(_context.Users.SingleOrDefault(u => u.Id == id), new UserEntity());

            if (user == null)
                return BadRequest("Пользователя с данными идентификатором не существует!");

            return Ok(user);
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
                _context.Users.Add(Helper.ConvertTo<User, UserEntity>(user, new User()));
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
                _context.Users.Update(Helper.ConvertTo<User, UserEntity>(user, new User()));
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
