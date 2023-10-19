using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolgaIT.Model.Model;
using VolgaIT.Model.ModelNoId;

namespace VolgaIT.Controllers.AdminControllers
{
    [Route("api/Admin/Transport")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminTransportController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<TransportNoId>> GetAllTransport()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public ActionResult<TransportNoId> GetTransport(long id) 
        {
            return Ok(id.ToString());
        }

        [HttpPost]
        public ActionResult AddTransport(TransportNoId transport)
        {
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult EditTransport(long id)
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
