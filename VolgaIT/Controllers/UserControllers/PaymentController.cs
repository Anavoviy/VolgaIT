using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VolgaIT.Controllers.UserControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        [HttpPost("Hesoyam/{accountId}")]
        [Authorize]
        public ActionResult Hesoyam(long accountId)
        {
            return Ok();
        }

    }
}
