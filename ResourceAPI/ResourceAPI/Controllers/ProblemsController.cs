using System;
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
            IProblemService problemProblemService, IAuthorService authorService)
        {
            _logger = logger;
            _context = context;
            _problemService = problemProblemService;
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
            var problems = _problemService.BrowseProblems(tags, query, newest, highest, page, out var totalPages);

            return new OkObjectResult(new
            {
                page,
                totalPages,
                problems
            });
        }


        [HttpGet]
        [Route("{id:int}")]
        public ActionResult Get(int id)
        {
            var problem = _problemService.ProblemWithAnswersById(id);
            if (problem == null) return StatusCode(404);
            return StatusCode(200, problem);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Post(Problem problem)
        {
            if (problem.ContentHtml == null) return StatusCode(400);
            if (problem.ContentHtml.Length > 1024 * 1024) return StatusCode(413);
            var author = _authorService.GetAuthor(1);
            if (author == null) return StatusCode(403);
            var problemId = _problemService.Create(1, problem);
            if (problemId == 0) return StatusCode(403);
            return StatusCode(201, new Problem {Id = problemId});
        }


        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public ActionResult Put(int id, Problem problem)
        {
            if (!_context.Problems.Any(p => p.Id == id)) return StatusCode(404);
            var initialProblem = _context.Problems.First(p => p.Id == id);

            initialProblem.Name = problem.Name;
            initialProblem.Content = problem.Content;
            initialProblem.Edited = DateTime.Now;

            _context.Problems.Update(initialProblem);
            _context.SaveChanges();

            return StatusCode(201);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public ActionResult Delete(int id)
        {
            if (!_context.Problems.Any(p => p.Id == id)) return StatusCode(404);
            var problem = _context.Problems.FirstOrDefault(p => p.Id == id);
            if (problem == null) return StatusCode(403);
            _context.Problems.Remove(problem);

            var answers = _context.Answers.Where(a => a.ProblemId == id);
            _context.Answers.RemoveRange(answers);

            var problemTags = _context.ProblemTags.Where(pt => pt.ProblemId == id);
            _context.ProblemTags.RemoveRange(problemTags);

            _context.SaveChanges();
            return StatusCode(201);
        }

        [HttpGet]
        [Route("{id}/points")]
        public ActionResult Points(int id)
        {
            var points = _context.Problems.First(problem => problem.Id == id).Points;
            return StatusCode(200, new {points});
        }


        [HttpPost]
        [Route("{id}/upvote")]
        [Authorize]
        public ActionResult UpvoteProblem(int id)
        {
            return VoteProblem(id, Vote.Upvote);
        }

        [HttpPost]
        [Route("{id}/downvote")]
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