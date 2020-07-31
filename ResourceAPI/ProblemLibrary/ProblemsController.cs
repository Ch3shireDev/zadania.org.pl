using System.Linq;
using System.Security.Claims;
using CommonLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

        private readonly IProblemService _problemService;

        private ILogger<ProblemsController> _logger;

        /// <summary>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        /// <param name="problemService"></param>
        /// <param name="authorService"></param>
        public ProblemsController(ILogger<ProblemsController> logger, IProblemDbContext context,
            IProblemService problemService, IAuthorService authorService)
        {
            _logger = logger;
            _context = context;
            _problemService = problemService;
            _authorService = authorService;
        }

        protected int AuthorId
        {
            get
            {
                return 1;
                var profileData = HttpContext?.Request.Headers["profile"] ?? string.Empty;
                var profile = JsonConvert.DeserializeObject<UserData>(profileData);
                var http_contextUser = HttpContext.User;
                var idClaim = http_contextUser.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
                if (idClaim == null) return 0;
                var id = _authorService.GetAuthor(idClaim.Value, profile);
                return id;
            }
        }

        /// <summary>
        ///     Zwraca listę problemów.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="page"></param>
        /// <param name="query"></param>
        /// <param name="newest"></param>
        /// <param name="highest"></param>
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
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult Get([FromRoute] int id)
        {
            var problem = _problemService.Get(id);
            if (problem == null) return NotFound();
            return Ok(problem);
        }

        /// <summary>
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult Post(Problem problem)
        {
            //var author = _authorService.GetAuthor(1);
            //if (author == null) return StatusCode(403);
            var problemId = _problemService.Create(problem, AuthorId);
            if (problemId == 0) return StatusCode(403);
            return StatusCode(201, new Problem {Id = problemId});
        }


        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="problem"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult Put(int id, Problem problem)
        {
            var result = _problemService.Edit(id, problem);
            if (!result) return BadRequest();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var result = _problemService.Delete(id);
            return Ok();
        }

        [HttpGet("{id}/points")]
        public ActionResult Points(int id)
        {
            var points = _context.Problems.First(problem => problem.Id == id).Points;
            return StatusCode(200, new {points});
        }


        [HttpPost("{id}/upvote")]
        [Authorize]
        public ActionResult UpvoteProblem(int id)
        {
            return VoteProblem(id, Vote.Upvote);
        }

        [HttpPost("{id}/downvote")]
        [Authorize]
        public ActionResult DownvoteProblem(int id)
        {
            return VoteProblem(id, Vote.Downvote);
        }

        [NonAction]
        public ActionResult VoteProblem(int id, Vote vote)
        {
            var problem = _problemService.Get(id);
            if (problem == null) return StatusCode(403);
            //var author = _authorService.GetAuthor(1);
            //if (author == null) return StatusCode(403);

            _problemService.VoteProblem(id, vote);
            return StatusCode(200);
        }

        [HttpGet("{problemId}/answers/{answerId}")]
        public ActionResult GetAnswer(int problemId, int answerId)
        {
            var answer = _problemService.GetAnswer(problemId, answerId);
            if (answer == null) return NotFound();
            return Ok(answer);
        }

        [HttpPost("{problemId}/answers")]
        public ActionResult PostAnswer(int problemId, Answer answer)
        {
            var answerId = _problemService.CreateAnswer(problemId, answer);
            if (answerId == 0) return Forbid();
            return Ok(new Answer {Id = answerId});
        }

        [HttpPut("{problemId}/answers/{answerId}")]
        public ActionResult PutAnswer(int problemId, int answerId, Answer answer)
        {
            var result = _problemService.EditAnswer(problemId, answerId, answer);
            if (result == false) return Forbid();
            return Ok();
        }

        [HttpDelete("{problemId}/answers/{answerId}")]
        public ActionResult DeleteAnswer(int problemId, int answerId)
        {
            var result = _problemService.DeleteAnswer(problemId, answerId);
            if (result == false) return Forbid();
            return Ok();
        }

        [HttpPatch("{problemId}/answers/{answerId}")]
        public ActionResult PatchAnswer(int problemId, int answerId, Answer answer)
        {
            _problemService.SetAnswerApproval(problemId, answerId, answer.IsApproved);
            return Ok();
        }
    }
}