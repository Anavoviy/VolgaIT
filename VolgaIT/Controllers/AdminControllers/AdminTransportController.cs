using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolgaIT.Model;

namespace VolgaIT.Controllers.AdminControllers
{
    [Route("api/Admin/Transport")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminTransportController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Transport>> GetAllTransport()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public ActionResult<Transport> GetTransport(long id) 
        {
            return Ok(id.ToString());
        }

        [HttpPost]
        public ActionResult AddTransport(Transport transport)
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
