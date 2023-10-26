using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VolgaIT.EntityDB;
using VolgaIT.Model.Entities;
using VolgaIT.Model.ModelNoId;
using VolgaIT.Model.ModelUniqueDataTransfers;
using VolgaIT.OtherClasses;

namespace VolgaIT.Controllers.UserControllers
{
    [Route("api/Transport")]
    [ApiController]
    public class TransportController : ControllerBase
    {
        private DataBaseContext _context;

        public TransportController(DataBaseContext context) => _context = context;

        [HttpGet("{id}")]
        public ActionResult<TransportNoId> GetTransportById(long id)
        {
            TransportEntity transport = _context.Transports.FirstOrDefault(t => t.Id == id);
            if (transport == null)
                return BadRequest("Не сущетсвует траспортного средства с данным идентификаторм!");

            return Ok(Helper.ConvertTo<TransportEntity, TransportNoId>(transport, new TransportEntity()));
        }


        [HttpPost]
        [Authorize]
        public ActionResult AddTransport(UnicTransport transport)
        {
            if (transport == null)
                return BadRequest("Был передан транспорт, у которого не заполнены данные");

            TransportEntity transportEntity = Helper.ConvertTo<UnicTransport, TransportEntity>(transport, new UnicTransport());
            
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            long userId = HelperWithJWT.instance.UserId(headers);

            transportEntity.OwnerId = userId;

            _context.Transports.Add(transportEntity);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize]
        public ActionResult UpdateTransport(long id, UnicTransport transport)
        {
            if (transport == null)
                ModelState.AddModelError("Null", "Был передан транспорт, у которого не заполнены данные!");
            if (_context.Transports.FirstOrDefault(t => t.Id == id) == null)
                ModelState.AddModelError("Id", "Траспортного средства с таким идентификатором не существует!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            long userId = HelperWithJWT.instance.UserId(headers);

            if (_context.Transports.FirstOrDefault(t => t.Id == id).OwnerId != userId)
                return BadRequest("Вы не являетесь владельцем данного транспортного средства!");

            TransportEntity transportEntity = _context.Transports.FirstOrDefault(t => t.Id == id);

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
        [Authorize]
        public ActionResult DeleteTransport(long id)
        {
            string headers = this.HttpContext.Request.Headers.Authorization.ToString();
            long userId = HelperWithJWT.instance.UserId(headers);

            if (_context.Transports.FirstOrDefault(t => t.Id == id) == null)
                return BadRequest("Траспортного средства с таким идентификатором не существует!");

            if (_context.Transports.FirstOrDefault(t => t.Id == id).OwnerId != userId)
                ModelState.AddModelError("User", "Вы не являетесь владельцем данного транспортного средства!");
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Transports.Remove(_context.Transports.FirstOrDefault(u => u.Id == id));
            _context.SaveChanges();

            return Ok();
        }
    }
}
