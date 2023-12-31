﻿using Microsoft.AspNetCore.Authorization;
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
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            if (_context.Users == null)
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
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            if (id == 0 || _context.Users.FirstOrDefault(u => u.Id == id) == null)
                ModelState.AddModelError("Id", "Пользователя с таким идентификатором не существует в системе!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User user = Helper.ConvertTo<UserEntity, User>(_context.Users.SingleOrDefault(u => u.Id == id), new UserEntity());

            return Ok(user);
        }

        [HttpPost]
        public ActionResult AddUser(UserNoId user)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            if (_context.Users.FirstOrDefault(u => u.Username == user.Username) != null)
                return BadRequest("Нельзя создать пользователя с username уже существующим в системе");

            _context.Users.Add(Helper.ConvertTo<UserNoId, UserEntity>(user, new UserNoId()));
            _context.SaveChanges();
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult EditUser(long id, UserNoId user) 
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            if (_context.Users.FirstOrDefault(u => u.Username == user.Username) != null)
                return BadRequest("Нельзя изменять username пользователя на уже существующий в системе");

           _context.Users.Update(Helper.ConvertTo<UserNoId, UserEntity>(user, new UserNoId()));
           _context.SaveChanges();
           return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(long id)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            if (id == 0 || _context.Users.FirstOrDefault(u => u.Id == id) == null)
                return BadRequest("Пользователя с таким идентификатором не существует в системе!");

            _context.Users.Remove(_context.Users.FirstOrDefault(u => u.Id == id));
            _context.SaveChanges();

            return Ok();
        }



    }
}
