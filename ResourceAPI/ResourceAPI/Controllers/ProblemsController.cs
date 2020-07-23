using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceAPI.ApiServices.Interfaces;
using ResourceAPI.Enums;
using ResourceAPI.Models.Problem;

namespace ResourceAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProblemsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        private readonly SqlContext _context;

        private readonly IProblemService _problemService;

        private ILogger<ProblemsController> _logger;

        public ProblemsController(ILogger<ProblemsController> logger, SqlContext context,
            IProblemService problemService, IAuthorService authorService)
        {
            _logger = logger;
            _context = context;
            _problemService = problemService;
            _authorService = authorService;
        }

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


        [HttpGet("{id:int}")]
        public ActionResult Get(int id)
        {
            var problem = _problemService.Get(1, id);
            if (problem == null) return NotFound();
            return Ok(problem);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Post(Problem problem)
        {
            var author = _authorService.GetAuthor(1);
            if (author == null) return StatusCode(403);
            var problemId = _problemService.Create(1, problem);
            if (problemId == 0) return StatusCode(403);
            return StatusCode(201, new Problem {Id = problemId});
        }


        [HttpPut("{id}")]
        [Authorize]
        public ActionResult Put(int id, Problem problem)
        {
            var result = _problemService.Edit(1, id, problem);
            if (!result) return BadRequest();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var result = _problemService.Delete(1, id);
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
            var problem = _context.Problems.FirstOrDefault(p => p.Id == id);
            if (problem == null) return StatusCode(403);
            var author = _authorService.GetAuthor(1);
            if (author == null) return StatusCode(403);
            var problemVote =
                _context.ProblemVotes.FirstOrDefault(pv => pv.AuthorId == author.Id && pv.ProblemId == id);
            if (problemVote == null)
            {
                problemVote = new ProblemVote {Author = author, Problem = problem};
                _context.ProblemVotes.Add(problemVote);
            }
            else
            {
                problemVote.Vote = problemVote.Vote == vote ? Vote.None : vote;
                _context.ProblemVotes.Update(problemVote);
            }

            _context.SaveChanges();

            problem.Points = _context.ProblemVotes.Where(pv => pv.ProblemId == problem.Id)
                .Select(pv => pv.Vote == Vote.Upvote ? 1 : pv.Vote == Vote.Downvote ? -1 : 0)
                .Sum();

            _context.Problems.Update(problem);

            _context.SaveChanges();

            return StatusCode(200, problemVote.Vote);
        }
    }
}