using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VolgaIT.EntityDB;
using VolgaIT.Model.Entities;
using VolgaIT.Model.Model;
using VolgaIT.Model.ModelNoId;
using VolgaIT.OtherClasses;

namespace VolgaIT.Controllers.AdminControllers
{
    [Route("api/Admin/Transport")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminTransportController : ControllerBase
    {
        private DataBaseContext _context;

        public AdminTransportController(DataBaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Transport>> GetAllTransport(int start = 0, int count = 0, string transportType = null)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            if (_context.Transports == null)
                return BadRequest("Транспортных средств нет в базе данных!");

            List<TransportEntity> transportsEntity = new List<TransportEntity>();
            if (transportType == "" || transportType == null)
            {
                if (count > 0)
                    transportsEntity = _context.Transports.Skip(start).Take(count).OrderBy(t => t.Id).ToList();
                else
                    transportsEntity = _context.Transports.Skip(start).OrderBy(t => t.Id).ToList();
            }
            else
            {
                if (count > 0)
                    transportsEntity = _context.Transports.Where(t => t.TransportType == transportType).Skip(start).Take(count).OrderBy(t => t.Id).ToList();
                else
                    transportsEntity = _context.Transports.Where(t => t.TransportType == transportType).Skip(start).OrderBy(t => t.Id).ToList();
            }

            if (transportsEntity == null || transportsEntity.Count == 0)
                return BadRequest("Траспортных средств по вашим параметрам нет в базе данных");

            List<Transport> transports = new List<Transport>();
            foreach (TransportEntity transportEntity in transportsEntity)
                transports.Add(Helper.ConvertTo<TransportEntity, Transport>(transportEntity, new TransportEntity()));

            return Ok(transports);
        }

        [HttpGet("{id}")]
        public ActionResult<TransportNoId> GetTransport(long id) 
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            if (_context.Transports == null)
                return BadRequest("Транспортных средств нет в базе данных!");

            TransportEntity transportEntity = _context.Transports.FirstOrDefault(t => t.Id == id);
            if (transportEntity == null)
                return BadRequest("Траспортного средства с таким идентификатором не существует!");


            return Ok(Helper.ConvertTo<TransportEntity, Transport>(transportEntity, new TransportEntity()));
        }

        [HttpPost]
        public ActionResult AddTransport(TransportNoId transport)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            if (transport == null)
                return BadRequest("Было передано траспортное средство, в котором не заполнены данные!");

            TransportEntity transportEntity = Helper.ConvertTo<TransportNoId, TransportEntity>(transport, new TransportNoId());

            _context.Transports.Add(transportEntity);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult EditTransport(long id, TransportNoId transport)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            if (transport == null)
                return BadRequest("Было передано траспортное средство, в котормо не заполнены данные!");

            TransportEntity transportEntity = _context.Transports.FirstOrDefault(t => t.Id == id);
            if (transportEntity == null)
                return BadRequest("Траспортного средства с таким идентификатором не существует!");

            transportEntity.OwnerId = transport.OwnerId;
            transportEntity.CanBeRented = transport.CanBeRented;
            transportEntity.TransportType = transport.TransportType;
            transportEntity.Model = transport.Model;
            transportEntity.Color = transport.Color;
            transportEntity.Identifier = transport.Identifier;
            transportEntity.Description = transport.Description;
            transportEntity.Latitude = transport.Latitude;
            transportEntity.Longitude = transport.Longitude;
            transportEntity.MinutePrice = transport.MinutePrice;
            transportEntity.DayPrice = transport.DayPrice;

            _context.Transports.Update(transportEntity);
            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteTransport(long id)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            if (!HelperWithJWT.instance.TokenIsValid(headers))
                return Unauthorized("Авторизуйтесь!");

            TransportEntity transportEntity = _context.Transports.FirstOrDefault(t => t.Id == id);
            if (transportEntity == null)
                return BadRequest("Траспортного средства с таким идентфикатором не сущетсвует!");

            _context.Transports.Remove(transportEntity);
            _context.SaveChanges();

            return Ok();
        }
    }
}
