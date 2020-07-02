using System;
using System.Collections.Generic;
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
        private readonly ILogger<ProblemsController> _logger;

        public ProblemsController(ILogger<ProblemsController> logger, SqlContext context)
        {
            _logger = logger;
            Context = context;
        }

        private SqlContext Context { get; }

        [HttpGet]
        public ActionResult Browse([FromQuery] string tags = null, [FromQuery] int page = 1)
        {
            var tag = tags;
            var problemsQuery = Context.Problems.AsQueryable();

            if (tags != null)
                problemsQuery = problemsQuery
                    .Where(p => p.ProblemTags.Select(pt => pt.Tag.Url)
                        .Any(t => t == tag));

            var resultQuery = problemsQuery
                .Include(p => p.Author)
                .Select(p => new Problem
                    {
                       Id= p.Id,
                        Title=p.Title,
                       Content= p.Content,
                        Created=p.Created,
                        Edited=p.Edited,
                        Author = new Author{Name=p.Author.Name,UserId= p.Author.UserId,Email= p.Author.Email, Id=p.Author.Id},
                        Points = p.ProblemVotes.Count(pv => pv.Vote == Vote.Upvote) -
                                 p.ProblemVotes.Count(pv => pv.Vote == Vote.Downvote),
                    Tags = p.ProblemTags.Select(pt => pt.Tag).ToArray(),
                    UserUpvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Upvote),
                    UserDownvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Downvote),
                    IsAnswered = p.Answers.Any(a=>a.IsApproved)
                }
                ).AsQueryable();

            var newest = resultQuery.OrderByDescending(r=>r.Points).AsQueryable();

            //var points = resultQuery.OrderByDescending(r => r.Points).AsQueryable();


            var num = resultQuery.Count();

            var lastRecordIndex = page * 10;
            var firstRecordIndex = lastRecordIndex - 10;

            var subQuery = newest;

            //var list = new List<Problem>();
            //foreach (var element in newest)
            //{
            //    list.Add(element);
            //}
            //foreach (var element in points)
            //{
            //    list.Add(element);
            //}

            //var subQuery = list.AsEnumerable();

            if (firstRecordIndex < num) subQuery = subQuery.Skip(firstRecordIndex);
            if (lastRecordIndex < num) subQuery = subQuery.Take(10);

            return StatusCode(200, new
            {
                pageNum = page,
                totalPages = num % 10 == 0 ? num / 10 : num / 10 + 1,
                problems = subQuery.ToArray()
            });
        }

        [HttpGet]
        [Route("search")]
        public ActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return StatusCode(204);
            if (!Context.Problems.Any()) return StatusCode(204);

            var problems = Context.Problems
                    .Include(p => p.Author)
                    .Select(p => new Problem
                        {
                            Id = p.Id,
                            Title = p.Title,
                            Content = p.Content,
                            Created = p.Created,
                            Edited = p.Edited,
                            Author = new Author { Name = p.Author.Name, UserId = p.Author.UserId, Email = p.Author.Email, Id = p.Author.Id },
                            Points = p.ProblemVotes.Count(pv => pv.Vote == Vote.Upvote) -
                                     p.ProblemVotes.Count(pv => pv.Vote == Vote.Downvote),
                            Tags = p.ProblemTags.Select(pt => pt.Tag).ToArray(),
                            UserUpvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Upvote),
                            UserDownvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Downvote),
                            IsAnswered = p.Answers.Any(a => a.IsApproved)
                    }
                    )
                    .Where(problem => problem.Content.Contains(query))
                .OrderByDescending(problem => problem.Id)
                .ToArray();

            return StatusCode(200, problems);
        }

        //[HttpGet]
        public ActionResult Get([FromQuery] int from, [FromQuery] int num)
        {
            if (!Context.Problems.Any()) return StatusCode(204);
            var max = Context.Problems.Select(problem => problem.Id).Max();
            var problems = Context.Problems
                .Include(p => p.ProblemTags)
                .Where(problem => problem.Id > max - num)
                .Select(p =>
                    new Problem
                    {
                        Id = p.Id,
                        Content = p.Content,
                        Title = p.Title,
                        Tags = p.ProblemTags.Select(pt => pt.Tag).ToArray(),
                        IsAnswered = p.Answers.Any(a=>a.IsApproved)
                    }
                )
                .ToArray()
                .Reverse()
                .ToArray();

            //var pcs = Context.ProblemTags.ToArray();
            //var tags = Context.Tags.ToArray();

            return StatusCode(200, problems);
        }

        [HttpGet]
        [Route("{id:int}")]
        public ActionResult Get(int id)
        {
            if (!Context.Problems.Any(p => p.Id == id)) return StatusCode(404);
            var problem = Context.Problems
                .Include(p => p.ProblemTags)
                .Select(p => new Problem
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Answers = p.Answers,
                    AuthorId = p.AuthorId,
                    Author = p.Author,
                    Created = p.Created,
                    Edited = p.Edited,
                    Tags = p.ProblemTags.Select(pt => pt.Tag).ToArray(),

                    Points = p.ProblemVotes.Count(pv => pv.Vote == Vote.Upvote) -
                             p.ProblemVotes.Count(pv => pv.Vote == Vote.Downvote),
                    UserUpvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Upvote),
                    UserDownvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Downvote)
                })
                .First(p => p.Id == id);

            problem.Author = problem.Author.Serializable();

            return StatusCode(200, problem);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Post(Problem problem)
        {
            if (problem.Content == null) return StatusCode(400);
            if (problem.Content.Length > 1024 * 1024) return StatusCode(413);
            var author = AuthorsController.GetAuthor(HttpContext, Context);
            if (author == null) return StatusCode(403);

            problem.ProblemTags = RefreshTags(problem).ToArray();

            problem.Created = DateTime.Now;
            problem.Author = author;
            problem.AuthorId = author.Id;

            author.Problems.Add(problem);
            Context.Authors.Update(author);
            Context.Problems.Add(problem);
            Context.SaveChanges();
            return StatusCode(201);
        }

        private IEnumerable<ProblemTag> RefreshTags(Problem problem)
        {
            return RefreshTags(problem, Context);
        }

        public static IEnumerable<ProblemTag> RefreshTags(Problem problem, SqlContext context)
        {
            if (problem.Tags == null) yield break;
            foreach (var tag in problem.Tags)
            {
                tag.Url = tag.GenerateUrl();
                var existing = context.Tags.Find(tag.Url) ?? tag;
                yield return new ProblemTag {Problem = problem, Tag = existing};
            }
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

            var answers = Context.Answers.Where(a => a.ParentId == id);
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

            return StatusCode(200, problemVote.Vote);
        }
    }
}