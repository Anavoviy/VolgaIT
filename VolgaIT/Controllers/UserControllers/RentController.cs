using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using VolgaIT.EntityDB;
using VolgaIT.Model.Entities;
using VolgaIT.Model.Model;
using VolgaIT.OtherClasses;

namespace VolgaIT.Controllers.UserControllers
{
    [Route("api/Rent")]
    [ApiController]
    public class RentController : ControllerBase
    {
        private DataBaseContext _context;

        public RentController(DataBaseContext context) 
        { 
            _context = context;
        }


        [HttpGet("Transport")]
        public ActionResult<List<Transport>> GetTransports(double lat = 0, double _long = 0, double radius = 0, string type = "")
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            List<TransportEntity> transportsEntity = _context.Transports.Where(t => t.TransportType == type &&
                                                                              (Math.Pow(lat - t.Latitude, 2) + Math.Pow(_long - t.Longitude, 2) <= Math.Pow(radius, 2)))
                                                                              .ToList();
            if (transportsEntity == null || transportsEntity.Count == 0)
                return BadRequest("Траспортных средств с заданными вами параметрами не найдено");

            List<Transport> transports = new List<Transport>();
            foreach (var transportEntity in transportsEntity)
                transports.Add(Helper.ConvertTo<TransportEntity, Transport>(transportEntity, new TransportEntity()));

            return Ok(transports);
        }

        [HttpGet("{rentId}")]
        [Authorize]
        public ActionResult<Rent> GetRendById(long rentId)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            RentEntity rentEntity = _context.Rents.FirstOrDefault(r => r.Id == rentId);

            long userId = HelperWithJWT.instance.UserId(headers);
            if (userId != rentEntity.UserId && userId != _context.Transports.FirstOrDefault(t => t.Id == rentEntity.TransportId).OwnerId)
                return BadRequest("Нет доступа");

            Rent rent = Helper.ConvertTo<RentEntity, Rent>(rentEntity, new RentEntity());
            return Ok(rent);
        }

        [HttpGet("MyHistory")]
        [Authorize]
        public ActionResult<List<Rent>> GetMyRentsHistory()
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            long userId = HelperWithJWT.instance.UserId(headers);

            List<RentEntity> entities = _context.Rents.Where(r => r.UserId == userId).ToList();
            if (entities == null || entities.Count == 0)
                return NoContent();

            List<Rent> rents = new List<Rent>();
            foreach (var entity in entities)
                rents.Add(Helper.ConvertTo<RentEntity, Rent>(entity, new RentEntity()));

            return Ok(rents);
        }

        [HttpGet("TransportHistory/{transportId}")]
        [Authorize]
        public ActionResult<List<Rent>> GetMyTransportHistory(long transportId)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            long userId = HelperWithJWT.instance.UserId(headers);

            if (_context.Transports.FirstOrDefault(t => t.Id == transportId).OwnerId != userId)
                return BadRequest();

            List<RentEntity> entities = _context.Rents.Where(r => r.TransportId == transportId).ToList();
            if (entities == null || entities.Count == 0)
                return NoContent();

            List<Rent> rents = new List<Rent>();
            foreach (var entity in entities)
                rents.Add(Helper.ConvertTo<RentEntity, Rent>(entity, new RentEntity()));

            return Ok(rents);
        }

        [HttpPost("New/{transportId}")]
        [Authorize]
        public ActionResult NewRentTransport(long transportId, string rentType)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            long userId = HelperWithJWT.instance.UserId(headers);

            if(rentType != "Minutes" && rentType != "Days")
                return BadRequest("Типы аренды бывают только: минуты (Minutes) и дни (Days)!");
            if (_context.Transports.FirstOrDefault(t => t.Id == transportId).OwnerId == userId)
                return BadRequest("нельзя брать в аренду своё траспортное средство!");

            DateTime dateTime = DateTime.Now;
            string dateTimeISO = dateTime.ToString("yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            RentEntity rentEntity = new RentEntity();
            rentEntity.TransportId = transportId;
            rentEntity.UserId = userId;
            rentEntity.PriceType = rentType;
            rentEntity.TimeStart = dateTimeISO;
            if (rentType == "Minutes")
                rentEntity.PriceOfUnit = (double)(_context.Transports.FirstOrDefault(t => t.Id == transportId).MinutePrice);
            else    
                rentEntity.PriceOfUnit = (double)(_context.Transports.FirstOrDefault(t => t.Id == transportId).DayPrice);

            _context.Rents.Add(rentEntity);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost("End/{rendtId}")]
        [Authorize]
        public ActionResult EndRentTransport(long rentId, double lat, double _long)
        {
            string headers = this.Request.Headers.Authorization.ToString();
            long userId = HelperWithJWT.instance.UserId(headers);
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            RentEntity rentEntity = _context.Rents.FirstOrDefault(r => r.Id == rentId);
            if (rentEntity == null)
                return BadRequest("Не существует аренды с таким идентификатором!");
            if (rentEntity.UserId != userId)
                return BadRequest("Нет доступа!");

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

                    UserEntity userEntity = _context.Users.FirstOrDefault(u => u.Id == userId);
            
                    if (userEntity.Balance < rentEntity.FinalPrice)
                        return BadRequest("Недостаточно средств!");        
            
                    userEntity.Balance = (double)(userEntity.Balance - rentEntity.FinalPrice);

                    _context.Users.Update(userEntity);
            

                2 вариант (при условии - можно уходить в отрицательный баланс

                    UserEntity userEntity = _context.Users.FirstOrDefault(u => u.Id == userId);
            
                    userEntity.Balance = (double)(userEntity.Balance - rentEntity.FinalPrice);

                    _context.Users.Update(userEntity);
            */

            _context.Transports.Update(transportEntity);
            _context.Rents.Update(rentEntity);

            _context.SaveChanges();

            return Ok();
        }
    }
}
