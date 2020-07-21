using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceAPI.ApiServices.Interfaces;

namespace ResourceAPI.Controllers
{
    [Route("api/v1/automated-exercises")]
    [ApiController]
    public class AutomatedExercisesController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;
        private readonly ILogger<AutomatedExercisesController> _logger;

        public AutomatedExercisesController(ILogger<AutomatedExercisesController> logger,
            IExerciseService exerciseService)
        {
            _logger = logger;
            _exerciseService = exerciseService;
        }

        [HttpGet]
        public ActionResult GetExercises()
        {
            var exercises = _exerciseService.Browse();
            return StatusCode(200, exercises);
        }
    }
}