using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExerciseLibrary
{
    /// <summary>
    /// </summary>
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

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetExercises()
        {
            var exercises = _exerciseService.Browse();
            return StatusCode(200, exercises);
        }

        /// <summary>
        /// </summary>
        /// <param name="exercise"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PostExercise(Exercise exercise)
        {
            _exerciseService.Create(exercise);
            return Ok();
        }
    }
}