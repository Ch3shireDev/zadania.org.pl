using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ResourceAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ILogger<ProblemsController> _logger;

        public TagsController(ILogger<ProblemsController> logger, SqlContext context)
        {
            _logger = logger;
            Context = context;
        }

        private SqlContext Context { get; }

        [HttpGet]
        public ActionResult GetTags()
        {
            var tags = Context.Tags.Select(t =>
                    new
                    {
                        t.Name,
                        t.Url,
                        t.ProblemTags.Count
                    })
                .OrderByDescending(t => t.Count)
                .Where(t => t.Count > 0)
                .Take(30)
                .ToArray();

            return StatusCode(200, tags);
        }
    }
}