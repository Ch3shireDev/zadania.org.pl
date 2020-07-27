using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceAPI.ApiServices.Interfaces;
using ResourceAPI.Models.Exercise;

namespace ResourceAPI.Controllers
{
    [Route("api/v1/exercises")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;
        private readonly ILogger<ExercisesController> _logger;

        public ExercisesController(ILogger<ExercisesController> logger,
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

        [HttpPost]
        public ActionResult PostExercise(Exercise exercise)
        {
            _exerciseService.Create(exercise);
            return Ok();
        }
    }
}