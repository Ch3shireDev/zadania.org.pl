using CommonLibrary.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExerciseLibrary
{
    /// <summary>
    ///     Zarządzanie ćwiczeniami automatycznymi.
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
        ///     Przegląda dostępne ćwiczenia.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetExercises()
        {
            var exercises = _exerciseService.Browse();
            return Ok(exercises);
        }

        /// <summary>
        ///     Tworzy nowe ćwiczenie.
        /// </summary>
        /// <param name="exercise"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PostExercise(Exercise exercise)
        {
            var result = _exerciseService.Create(exercise);
            if (result == 0) return Forbid();
            return Ok(new Exercise {Id = result});
        }

        /// <summary>
        ///     Zwraca ćwiczenie o zadanym id.
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <returns></returns>
        [HttpGet("{exerciseId}")]
        public ActionResult GetExercise(int exerciseId)
        {
            var result = _exerciseService.Get(exerciseId);
            if (result == null) return NotFound();
            return Ok(result);
        }


        /// <summary>
        ///     Zmienia ćwiczenie o zadanym id na wysłane.
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="exercise"></param>
        /// <returns></returns>
        [HttpPut("{exerciseId}")]
        public ActionResult EditExercise(int exerciseId, Exercise exercise)
        {
            var result = _exerciseService.Edit(exerciseId, exercise);
            if (result == false) return Forbid();
            return Ok();
        }

        /// <summary>
        ///     Usuwa ćwiczenie o zadanym id.
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <returns></returns>
        [HttpDelete("{exerciseId}")]
        public ActionResult DeleteExercise(int exerciseId)
        {
            var result = _exerciseService.Delete(exerciseId);
            if (result == false) return Forbid();
            return Ok();
        }

        /// <summary>
        ///     Zwraca skrypt z podanego ćwiczenia o zadanym id.
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="scriptId"></param>
        /// <returns></returns>
        [HttpGet("{exerciseId}/scripts/{scriptId}")]
        public ActionResult GetScript(int exerciseId, int scriptId)
        {
            var script = _exerciseService.GetScript(exerciseId, scriptId);
            if (script == null) return NotFound();
            return Ok(script);
        }

        /// <summary>
        ///     Tworzy nowy skrypt w podanym ćwiczeniu o zadanej strukturze.
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        [HttpPost("{exerciseId}/scripts")]
        public ActionResult PostScript(int exerciseId, Script script)
        {
            var scriptId = _exerciseService.CreateScript(exerciseId, script);
            if (scriptId == 0) return Forbid();
            return Ok(new Script {Id = scriptId});
        }

        /// <summary>
        ///     Zmienia skrypt z danego ćwiczenia o zadanym id na skrypt o zadanej strukturze.
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="scriptId"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        [HttpPut("{exerciseId}/scripts/{scriptId}")]
        public ActionResult EditScript(int exerciseId, int scriptId, Script script)
        {
            var result = _exerciseService.EditScript(exerciseId, scriptId, script);
            if (result == false) return Forbid();
            return Ok();
        }

        /// <summary>
        ///     Kasuje skrypt z danego ćwiczenia o zadanym id.
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <param name="scriptId"></param>
        /// <returns></returns>
        [HttpDelete("{exerciseId}/scripts/{scriptId}")]
        public ActionResult DeleteScript(int exerciseId, int scriptId)
        {
            var result = _exerciseService.DeleteScript(exerciseId, scriptId);
            if (result == false) return Forbid();
            return Ok();
        }
    }
}