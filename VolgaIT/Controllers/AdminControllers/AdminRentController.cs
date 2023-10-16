using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolgaIT.Model;

namespace VolgaIT.Controllers.AdminControllers
{
    [Route("api/Admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminRentController : ControllerBase
    {

        [HttpGet("Rent/{rentId}")]
        public ActionResult<Rent> GetRentById(long rentId)
        {
            return Ok(rentId.ToString());
        }

        [HttpGet("UserHistory/{userId}")]
        public ActionResult<List<Rent>> GetRentsByUserId(long userId)
        {
            return Ok();
        }

        [HttpGet("TransportHistory/{transportId}")]
        public ActionResult<List<Transport>> GetTransportById(long transportId)
        {
            return Ok();
        }


        [HttpPost("Rent")]
        public ActionResult AddRent(Rent rent) 
        {
            return Ok();
        }

        [HttpPost("Rent/End/{rentId}")]
        public ActionResult EndRent(long rentId)
        {
            return Ok();
        }


        [HttpPut("Rent/{id}")]
        public ActionResult EditRent(long id)
        {
            return Ok();
        }


        [HttpDelete("Rent/{rentId}")]
        public ActionResult DeleteRent(long id)
        {
            return Ok();
        }

    }
}
