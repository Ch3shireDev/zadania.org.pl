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

        public ProblemsController(ILogger<ProblemsController> logger, SqlContext context,
            IProblemService problemProblemService, IAuthorService authorService)
        {
            Logger = logger;
            Context = context;
            ProblemService = problemProblemService;
            _authorService = authorService;
        }

        private ILogger<ProblemsController> Logger { get; }

        private SqlContext Context { get; }

        private IProblemService ProblemService { get; }

        [HttpGet]
        public OkObjectResult Browse(
            [FromQuery] string tags = null,
            [FromQuery] int page = 1,
            [FromQuery] string query = null,
            [FromQuery] bool newest = false,
            [FromQuery] bool highest = false
        )
        {
            var problems = ProblemService.BrowseProblems(tags, query, newest, highest, page, out var totalPages);

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
            var problem = ProblemService.ProblemWithAnswersById(id);
            if (problem == null) return StatusCode(404);
            return StatusCode(200, problem);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Post(Problem problem)
        {
            if (problem.ContentHtml == null) return StatusCode(400);
            if (problem.ContentHtml.Length > 1024 * 1024) return StatusCode(413);
            var author = _authorService.GetAuthor(HttpContext);
            if (author == null) return StatusCode(403);
            var result = ProblemService.AddProblem(problem, author);
            //var result = _context.AddProblem(problem, author);
            if (!result) return StatusCode(403);
            Context.SaveChanges();
            return StatusCode(201);
        }


        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public ActionResult Put(int id, Problem problem)
        {
            if (!Context.Problems.Any(p => p.Id == id)) return StatusCode(404);
            var initialProblem = Context.Problems.First(p => p.Id == id);

            initialProblem.Title = problem.Title;
            initialProblem.Content = problem.Content;
            initialProblem.Edited = DateTime.Now;

            Context.Problems.Update(initialProblem);
            Context.SaveChanges();

            return StatusCode(201);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public ActionResult Delete(int id)
        {
            if (!Context.Problems.Any(p => p.Id == id)) return StatusCode(404);
            var problem = Context.Problems.FirstOrDefault(p => p.Id == id);
            if (problem == null) return StatusCode(403);
            Context.Problems.Remove(problem);

            var answers = Context.Answers.Where(a => a.ProblemId == id);
            Context.Answers.RemoveRange(answers);

            var problemTags = Context.ProblemTags.Where(pt => pt.ProblemId == id);
            Context.ProblemTags.RemoveRange(problemTags);

            Context.SaveChanges();
            return StatusCode(201);
        }

        [HttpGet]
        [Route("{id}/points")]
        public ActionResult Points(int id)
        {
            var points = Context.Problems.First(problem => problem.Id == id).Points;
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
            var problem = Context.Problems.FirstOrDefault(p => p.Id == id);
            if (problem == null) return StatusCode(403);
            var author = _authorService.GetAuthor(HttpContext);
            if (author == null) return StatusCode(403);
            var problemVote = Context.ProblemVotes.FirstOrDefault(pv => pv.AuthorId == author.Id && pv.ProblemId == id);
            if (problemVote == null)
            {
                problemVote = new ProblemVote {Author = author, Problem = problem};
                Context.ProblemVotes.Add(problemVote);
            }
            else
            {
                problemVote.Vote = problemVote.Vote == vote ? Vote.None : vote;
                Context.ProblemVotes.Update(problemVote);
            }

            Context.SaveChanges();

            problem.Points = Context.ProblemVotes.Where(pv => pv.ProblemId == problem.Id)
                .Select(pv => pv.Vote == Vote.Upvote ? 1 : pv.Vote == Vote.Downvote ? -1 : 0)
                .Sum();

            Context.Problems.Update(problem);

            Context.SaveChanges();

            return StatusCode(200, problemVote.Vote);
        }
    }
}