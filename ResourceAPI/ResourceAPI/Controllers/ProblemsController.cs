using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResourceAPI.Models;

namespace ResourceAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProblemsController : ControllerBase
    {
        private ILogger<ProblemsController> Logger { get; }

        public ProblemsController(ILogger<ProblemsController> logger, SqlContext context)
        {
            Logger = logger;
            Context = context;
        }

        private SqlContext Context { get; }

        [HttpGet]
        public ActionResult Browse(
            [FromQuery] string tags = null, 
            [FromQuery] int page = 1, 
            [FromQuery] string query=null,
            [FromQuery] bool newest=false,
            [FromQuery] bool highest=false
            )
        {
            var tag = tags;
            var problemsQuery = Context.Problems.AsQueryable();

            if (tags != null)
                problemsQuery = problemsQuery
                    .Where(p => p.ProblemTags.Select(pt => pt.Tag.Url).Any(t => t == tag));

            if (query != null) problemsQuery = problemsQuery.Where(p => p.Content.Contains(query) || p.ProblemTags.Any(pt=>pt.Tag.Name.Contains(query)));
            

            var resultQuery = problemsQuery.AsQueryable();

            var linksQuery = resultQuery;

            if (newest)
            {
                linksQuery= linksQuery
                    .OrderByDescending(p => p.Created)
                    .AsQueryable();
            }

            if (highest)
            {
                linksQuery = linksQuery.OrderByDescending(p => p.Points).AsQueryable();
            }

            var num = resultQuery.Count();

            var lastRecordIndex = page * 10;
            var firstRecordIndex = lastRecordIndex - 10;

            var subQuery = linksQuery;

            if (firstRecordIndex < num) subQuery = subQuery.Skip(firstRecordIndex);
            if (lastRecordIndex < num) subQuery = subQuery.Take(10);


            return StatusCode(200, new
            {
                page,
                totalPages = num % 10 == 0 ? num / 10 : num / 10 + 1,
                problemLinks = subQuery.ToList().Select(p => $"/api/v1/problems/{p.Id}")
            });
        }


        [HttpGet]
        [Route("{id:int}")]
        public ActionResult Get(int id)
        {
            if (!Context.Problems.Any(p => p.Id == id)) return StatusCode(404);
            var problem = Context.Problems
                .Include(p => p.Answers)
                .Select(p => new Problem
                {
                    Id = p.Id,
                    Title = p.Title,
                    ContentHtml = p.ContentHtml,
                    Content = p.Content,
                    AuthorId = p.AuthorId,
                    Author = p.Author,
                    Answers = p.Answers.Select(a => new Answer
                    {
                        Id = a.Id,
                        Content = a.Content,
                        AuthorId = a.AuthorId,
                        FileData = a.FileData,
                        Author = new Author {Name = a.Author.Name, Email = a.Author.Email, Id = a.Author.Id},
                        Points = a.Points
                    }).ToList(),
                    Created = p.Created,
                    Edited = p.Edited,
                    FileData = p.FileData,
                    Tags = p.ProblemTags.Select(pt => new Tag {Name = pt.Tag.Name, Url = pt.Tag.Url}).ToArray(),
                    Points = p.Points,
                    UserUpvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Upvote),
                    UserDownvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Downvote)
                })
                .First(p => p.Id == id);

            problem.Author = problem.Author.Serializable();
            problem.IsAnswered = Context.Answers.Where(a => a.ProblemId == problem.Id).Any(a => a.IsApproved);

            return StatusCode(200, problem.Render());
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