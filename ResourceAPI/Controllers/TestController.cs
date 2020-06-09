using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ResourceAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [Route("public")]
        [HttpGet]
        public ActionResult GetPublic()
        {
            return StatusCode(200, "public");
        }

        [Authorize]
        [Route("private")]
        [HttpGet]
        public ActionResult GetPrivate()
        {
            return StatusCode(200, "private message");
        }
    }
}