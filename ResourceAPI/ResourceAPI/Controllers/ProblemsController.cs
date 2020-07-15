using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceAPI.Models;
using ResourceAPI.Services;

namespace ResourceAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProblemsController : ControllerBase
    {
        public ProblemsController(ILogger<ProblemsController> logger, SqlContext context,
            IProblemService problemProblemService)
        {
            Logger = logger;
            Context = context;
            ProblemService = problemProblemService;
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
            var tag = tags;
            var problemsQuery = Context.Problems.AsQueryable();

            if (tags != null)
                problemsQuery = problemsQuery
                    .Where(p => p.ProblemTags.Select(pt => pt.Tag.Url).Any(t => t == tag));

            if (query != null)
                problemsQuery = problemsQuery.Where(p =>
                    p.Content.Contains(query) || p.ProblemTags.Any(pt => pt.Tag.Name.Contains(query)));


            var resultQuery = problemsQuery.AsQueryable();

            var linksQuery = resultQuery;

            if (newest)
                linksQuery = linksQuery
                    .OrderByDescending(p => p.Created)
                    .AsQueryable();

            if (highest) linksQuery = linksQuery.OrderByDescending(p => p.Points).AsQueryable();

            var num = resultQuery.Count();

            var lastRecordIndex = page * 10;
            var firstRecordIndex = lastRecordIndex - 10;

            var subQuery = linksQuery;

            if (firstRecordIndex < num) subQuery = subQuery.Skip(firstRecordIndex);
            if (lastRecordIndex < num) subQuery = subQuery.Take(10);

            var problems = subQuery.Select(p => new Problem {Id = p.Id}).ToArray()
                    .Select(p => ProblemService.ProblemById(p.Id)).ToArray()
                ;

            return new OkObjectResult(new
            {
                page,
                totalPages = num % 10 == 0 ? num / 10 : num / 10 + 1,
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
            var author = AuthorsController.GetAuthor(HttpContext, Context);
            if (author == null) return StatusCode(403);
            var result = Context.AddProblem(problem, author);
            if (result) Context.SaveChanges();
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
            var author = AuthorsController.GetAuthor(HttpContext, Context);
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