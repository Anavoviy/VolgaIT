using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VolgaIT.Controllers.UserControllers
{
    [Route("api/Rent")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RentController : ControllerBase
    {

        [HttpGet("Transport")]
        public ActionResult<string> GetTransports(double lat, double _long, double radius, string type)
        {
            return Ok();
        }

        [HttpGet("{rentId}")]
        public ActionResult<string> GetTransportById(long rentId)
        {
            return Ok();
        }

        [HttpGet("MyHistory")]
        public ActionResult<string> GetMyRentsHistory()
        {
            return Ok();
        }

        [HttpGet("TransportHistory/{transportId}")]
        public ActionResult<string> GetMyTransportHistory(long transportId)
        {
            return Ok();
        }

        [HttpPost("New/{transportId}")]
        public ActionResult NewRentTransport(long transportId, string rentType)
        {
            return Ok();
        }

        [HttpPost("End/{rendtId}")]
        public ActionResult EndRentTransport(long rentId, double lat, double _long)
        {
            return Ok();
        }
    }
}
