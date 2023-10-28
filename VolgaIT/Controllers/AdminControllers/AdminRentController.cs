using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Globalization;
using VolgaIT.EntityDB;
using VolgaIT.Model.Entities;
using VolgaIT.Model.Model;
using VolgaIT.Model.ModelNoId;
using VolgaIT.OtherClasses;

namespace VolgaIT.Controllers.AdminControllers
{
    [Route("api/Admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminRentController : ControllerBase
    {
        private DataBaseContext _context;
        
        public AdminRentController(DataBaseContext context)
        {
            _context = context;
        }

        [HttpGet("Rent/{rentId}")]
        public ActionResult<Rent> GetRentById(long rentId)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            RentEntity rentEntity = _context.Rents.FirstOrDefault(r => r.Id == rentId);
            if (rentEntity == null)
                return NoContent();

            Rent rent = Helper.ConvertTo<RentEntity, Rent>(rentEntity, new RentEntity());

            return Ok(rent);
        }

        [HttpGet("UserHistory/{userId}")]
        public ActionResult<List<Rent>> GetRentsByUserId(long userId)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            List<RentEntity> rentsEntity = _context.Rents.Where(r => r.UserId == userId).ToList();

            if(rentsEntity == null || rentsEntity.Count == 0) 
                return NoContent();

            List<Rent> rents = new List<Rent>();
            foreach (var entity in rentsEntity)
                rents.Add(Helper.ConvertTo<RentEntity, Rent>(entity, new RentEntity()));

            return Ok(rents);
        }

        [HttpGet("TransportHistory/{transportId}")]
        public ActionResult<List<Rent>> GetTransportById(long transportId)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            List<RentEntity> rentsEntity = _context.Rents.Where(r => r.TransportId == transportId).ToList();

            if (rentsEntity == null || rentsEntity.Count == 0)
                return NoContent();

            List<Rent> rents = new List<Rent>();
            foreach (var entity in rentsEntity)
                rents.Add(Helper.ConvertTo<RentEntity, Rent>(entity, new RentEntity()));

            return Ok(rents);
        }


        [HttpPost("Rent")]
        public ActionResult AddRent(Rent rent)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            if (rent == null)
                return BadRequest("Вы передали незаполненную данными аренду!");

            if (rent.TransportId == 0)
                ModelState.AddModelError("TransportId", "Не указан идентификатор транспорта!");
            if (rent.UserId == 0)
                ModelState.AddModelError("UserId", "Не указан идентификатор пользователя!");
            if (rent.TimeStart == "")
                ModelState.AddModelError("TimeStart", "Не указано время начала аренды!");
            DateTime datetime;
            if (!DateTime.TryParseExact(rent.TimeStart, "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out datetime))
                ModelState.AddModelError("TimeStart", "Некорректно указаны дата и время начало (требуемый формат: ГГГГ-ММ-ДДTЧЧ:ММ:СС)!");
            if (rent.PriceType != "Minutes" && rent.PriceType != "Days")
                ModelState.AddModelError("PriceType", "Существует всего два типа аренды: поминутно (Minutes) и подневно (Days)!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            RentEntity rentEntity = Helper.ConvertTo<Rent, RentEntity>(rent, new Rent());
            if (rent.PriceType == "Minutes")
                rentEntity.PriceOfUnit = (double)(_context.Transports.FirstOrDefault(t => t.Id == rent.TransportId).MinutePrice);
            else
                rentEntity.PriceOfUnit = (double)(_context.Transports.FirstOrDefault(t => t.Id == rent.TransportId).DayPrice);

            _context.Rents.Add(rentEntity);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost("Rent/End/{rentId}")]
        public ActionResult EndRent(long rentId, double lat, double _long)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            RentEntity rentEntity = _context.Rents.FirstOrDefault(r => r.Id == rentId);
            if (rentEntity == null)
                return BadRequest("Не существует аренды с таким идентификатором!");

            TransportEntity transportEntity = _context.Transports.FirstOrDefault(t => t.Id == rentEntity.TransportId);
            transportEntity.Latitude = lat;
            transportEntity.Longitude = _long;

            DateTime dateTimeEnd = DateTime.Now;
            string dateTimeEndISO = dateTimeEnd.ToString("yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            rentEntity.TimeEnd = dateTimeEndISO;

            DateTime dateTimeStart = DateTime.ParseExact(rentEntity.TimeStart, "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            TimeSpan ts = dateTimeEnd.Subtract(dateTimeStart);
            long difference;
            if (rentEntity.PriceType == "Minutes")
                difference = (long)Math.Round(ts.TotalMinutes);
            else
                difference = (long)Math.Round(ts.TotalDays);

            rentEntity.FinalPrice = difference * rentEntity.PriceOfUnit;
            /*
            Код для списывания средств с аккаунта:

                1 вариант (при условии - если недостаточно средств, то мы не даём закрыть аренду)

                    UserEntity userEntity = _context.Users.FirstOrDefault(u => u.Id == rentEntity.UserId);
            
                    if (userEntity.Balance < rentEntity.FinalPrice)
                        return BadRequest("Недостаточно средств!");        
            
                    userEntity.Balance = (double)(userEntity.Balance - rentEntity.FinalPrice);

                    _context.Users.Update(userEntity);
            

                2 вариант (при условии - можно уходить в отрицательный баланс)

                    UserEntity userEntity = _context.Users.FirstOrDefault(u => u.Id == userId);
            
                    userEntity.Balance = (double)(userEntity.Balance - rentEntity.FinalPrice);

                    _context.Users.Update(userEntity);
            */

            _context.Transports.Update(transportEntity);
            _context.Rents.Update(rentEntity);

            _context.SaveChanges();

            return Ok();
        }


        [HttpPut("Rent/{id}")]
        public ActionResult EditRent(long id, Rent rent)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            RentEntity rentEntity = _context.Rents.FirstOrDefault(t => t.Id == id);

            if (rent == null)
                return BadRequest("Вы передали незаполненную данными аренду!");

            if (rent.TransportId == 0)
                ModelState.AddModelError("TransportId", "Не указан идентификатор транспорта!");
            if (rent.UserId == 0)
                ModelState.AddModelError("UserId", "Не указан идентификатор пользователя!");
            if (rent.TimeStart == "")
                ModelState.AddModelError("TimeStart", "Не указано время начала аренды!");
            DateTime datetime;
            if (!DateTime.TryParseExact(rent.TimeStart, "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out datetime))
                ModelState.AddModelError("TimeStart", "Некорректно указаны дата и время начало (требуемый формат: ГГГГ-ММ-ДДTЧЧ:ММ:СС)!");
            if (rent.PriceType != "Minutes" && rent.PriceType != "Days")
                ModelState.AddModelError("PriceType", "Существует всего два типа аренды: поминутно (Minutes) и подневно (Days)!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            rentEntity.TransportId = rent.TransportId;
            rentEntity.UserId = rent.UserId;
            rentEntity.TimeStart = rent.TimeStart;
            if(rent.TimeEnd != "" && rent.TimeEnd != null)
                rentEntity.TimeEnd = rent.TimeEnd;
            rentEntity.PriceType = rent.PriceType;
            if(rent.FinalPrice != 0)
                rentEntity.FinalPrice = rent.FinalPrice;
            if (rent.PriceType == "Minutes")
                rentEntity.PriceOfUnit = (double)(_context.Transports.FirstOrDefault(t => t.Id == rent.TransportId).MinutePrice);
            else
                rentEntity.PriceOfUnit = (double)(_context.Transports.FirstOrDefault(t => t.Id == rent.TransportId).DayPrice);

            _context.Rents.Update(rentEntity);
            _context.SaveChanges();

            return Ok();
        }


        [HttpDelete("Rent/{rentId}")]
        public ActionResult DeleteRent(long id)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            RentEntity rent = _context.Rents.FirstOrDefault(t => t.Id == id);

            if (rent == null)
                return BadRequest("Аренды с таким идентификатором не существует!");

            _context.Rents.Remove(rent);
            _context.SaveChanges();

            return Ok();
        }

    }
}
