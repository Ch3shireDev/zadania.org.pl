using CommonLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ProblemLibrary
{
    /// <summary>
    ///     Problemy otwarte zamieszczane przez użytkowników.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProblemsController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly IProblemDbContext _context;
        private readonly ILogger<ProblemsController> _logger;
        private readonly IProblemService _problemService;

        /// <summary>
        ///     Konstruktor kontrolera.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        /// <param name="problemService"></param>
        /// <param name="authorService"></param>
        public ProblemsController(ILogger<ProblemsController> logger, IProblemDbContext context,
            IProblemService problemService, IAuthorService authorService = null)
        {
            _logger = logger;
            _context = context;
            _problemService = problemService;
            _authorService = authorService;
        }

        protected int AuthorId => Tools.GetAuthorId(_authorService, HttpContext);

        /// <summary>
        ///     Zwraca listę problemów spełniających dane warunki.
        /// </summary>
        /// <param name="tags">Tag problemu.</param>
        /// <param name="page">Nr strony.</param>
        /// <param name="query">Zapytanie.</param>
        /// <param name="newest">Czy sortować po najnowszych.</param>
        /// <param name="highest">Czy sortować po najwyżej punktowanych.</param>
        /// <returns></returns>
        [HttpGet]
        public OkObjectResult Browse(
            [FromQuery] string tags = null,
            [FromQuery] int page = 1,
            [FromQuery] string query = null,
            [FromQuery] bool newest = false,
            [FromQuery] bool highest = false
        )
        {
            var problems = _problemService.BrowseProblems(page, out var totalPages, tags, query, newest, highest);

            return new OkObjectResult(new
            {
                page,
                totalPages,
                problems
            });
        }

        /// <summary>
        ///     Zwraca problem o podanym id.
        /// </summary>
        /// <param name="id">Id problemu.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var problem = _problemService.Get(id);
            if (problem == null) return NotFound();
            return Ok(problem.ToView());
        }

        /// <summary>
        ///     Tworzy nowy problem na podstawie danych wysłanych przez użytkownika.
        /// </summary>
        /// <param name="problem">Dane nowego problemu.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult Post(ProblemUserModel problem)
        {
            var newProblem = _problemService.Create(problem.ToModel(), AuthorId);
            if (newProblem == null) return StatusCode(403);
            return StatusCode(201, newProblem.ToView());
        }


        /// <summary>
        ///     Zmienia wartości problemu na podstawie danych podanych przez użytkownika.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="problem"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult Put(int id, ProblemUserModel problem)
        {
            var result = _problemService.Edit(problem.ToModel(), id, AuthorId);
            if (!result) return BadRequest();
            return Ok();
        }

        /// <summary>
        ///     Usuwa dany problem na podstawie podanego identyfikatora.
        /// </summary>
        /// <param name="id">Identyfikator problemu.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var result = _problemService.Delete(id, AuthorId);
            if (result == false) return NotFound();
            return Ok();
        }


        /// <summary>
        ///     Zwraca odpowiedź o podanego problemu o podanym identyfikatorze.
        /// </summary>
        /// <param name="problemId">Identyfikator problemu.</param>
        /// <param name="answerId">Identyfikator odpowiedzi.</param>
        /// <returns>Odpowiedź.</returns>
        [HttpGet("{problemId}/answers/{answerId}")]
        public ActionResult GetAnswer(int problemId, int answerId)
        {
            var answer = _problemService.GetAnswer(problemId, answerId);
            if (answer == null) return NotFound();
            return Ok(answer.ToView());
        }

        /// <summary>
        ///     Tworzy nową odpowiedź dla wybranego problemu.
        /// </summary>
        /// <param name="problemId">Identyfikator problemu.</param>
        /// <param name="answer">Odpowiedź.</param>
        /// <returns></returns>
        [HttpPost("{problemId}/answers")]
        public ActionResult PostAnswer(int problemId, AnswerUserModel answer)
        {
            var answerId = _problemService.CreateAnswer(problemId, answer.ToModel());
            if (answerId == 0) return Forbid();
            return Ok(new Answer {Id = answerId});
        }

        /// <summary>
        ///     Aktualizuje odpowiedź na podstawie podanych przez użytkownika danych.
        /// </summary>
        /// <param name="problemId">Identyfikator problemu.</param>
        /// <param name="answerId">Identyfikator odpowiedzi.</param>
        /// <param name="answer">Odpowiedź.</param>
        /// <returns></returns>
        [HttpPut("{problemId}/answers/{answerId}")]
        public ActionResult PutAnswer(int problemId, int answerId, AnswerUserModel answer)
        {
            var result = _problemService.EditAnswer(problemId, answerId, answer.ToModel());
            if (result == false) return Forbid();
            return Ok();
        }

        /// <summary>
        ///     Kasuje odpowiedź o danym identyfikatorze.
        /// </summary>
        /// <param name="problemId">Identyfikator problemu.</param>
        /// <param name="answerId">Identyfikator odpowiedzi.</param>
        /// <returns></returns>
        [HttpDelete("{problemId}/answers/{answerId}")]
        [Authorize]
        public ActionResult DeleteAnswer(int problemId, int answerId)
        {
            var result = _problemService.DeleteAnswer(problemId, answerId, AuthorId);
            if (result == false) return Forbid();
            return Ok();
        }

        /// <summary>
        ///     Aktualizuje wybrane pola odpowiedzi.
        /// </summary>
        /// <param name="problemId">Identyfikator problemu.</param>
        /// <param name="answerId">Identyfikator odpowiedzi.</param>
        /// <param name="answer">Odpowiedź.</param>
        /// <returns></returns>
        [HttpPatch("{problemId}/answers/{answerId}")]
        public ActionResult PatchAnswer(int problemId, int answerId, AnswerUserModel answer)
        {
            _problemService.SetAnswerApproval(problemId, answerId, answer.IsApproved);
            return Ok();
        }
    }
}