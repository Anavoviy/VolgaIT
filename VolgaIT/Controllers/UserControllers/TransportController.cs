using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VolgaIT.Controllers.UserControllers
{
    [Route("api/Transport")]
    [ApiController]
    public class TransportController : ControllerBase
    {


        [HttpGet("{id}")]
        public ActionResult GetTransportById(long id)
        {

            return Ok();
        }

        [HttpPost]
        public ActionResult AddTransport(bool canBeRented, string model, string color, string identifier, string description, double latitude, double longitude, double minutePrice, double dayPrice)
        {
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult UpdateTransport(long id, bool canBeRented, string model, string color, string identifier, string description, double latitude, double longitude, double minutePrice, double dayPrice)
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteTransport(long id)
        {
            return Ok();
        }
    }
}
