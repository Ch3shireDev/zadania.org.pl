using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ResourceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly ILogger<ProblemsController> _logger;

        public ExercisesController(ILogger<ProblemsController> logger, SqlContext context)
        {
            _logger = logger;
            Context = context;
        }

        private SqlContext Context { get; }
    }
}