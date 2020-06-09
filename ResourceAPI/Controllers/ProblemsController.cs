using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceAPI.Models;

namespace ResourceAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProblemsController : ControllerBase
    {
        private readonly ILogger<ProblemsController> _logger;

        public ProblemsController(ILogger<ProblemsController> logger, DatabaseContext context)
        {
            _logger = logger;
            Context = context;
        }

        private DatabaseContext Context { get; }

        [HttpGet]
        public ActionResult Get()
        {
            var problems = Context.Problems.ToArray().Reverse().ToList();
            foreach (var problem in problems)
            {
                if (problem.Author == null) continue;
                problem.Author.Problems = null;
                problem.Author.Answers = null;
            }

            return StatusCode(200, problems);
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult Get(int id)
        {
            if (!Context.Problems.Any(p => p.Id == id)) return StatusCode(404);
            var problem = Context.Problems.First(p => p.Id == id);

            var author = AuthorsController.GetAuthor(HttpContext, Context);
            if (author != null) problem.UserVote = author.GetVote(Context, problem);

            if (problem.AuthorId != 0)
            {
                problem.Author = Context.Authors.FirstOrDefault(a => a.Id == problem.AuthorId);
                if (problem.Author != null)
                {
                    problem.Author.Problems = null;
                    problem.Author.Answers = null;
                }
            }

            var answers = Context.Answers.Where(answer => answer.Parent == problem).ToArray().Reverse().ToList();
            foreach (var answer in answers)
            {
                answer.Parent = null;
                answer.Author.Answers = null;
                answer.Author.Problems = null;
            }

            problem.Answers = answers;

            return StatusCode(200, problem);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Post(Problem problem)
        {
            var author = AuthorsController.GetAuthor(HttpContext, Context);
            if (author == null) return StatusCode(401);
            problem.Created = DateTime.Now;
            problem.Author = author;
            problem.AuthorId = author.Id;
            author.Problems.Add(problem);
            Context.Authors.Update(author);
            Context.Problems.Add(problem);
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
            var problem = Context.Problems.First(p => p.Id == id);
            Context.Problems.Remove(problem);
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

        [HttpPut]
        [Route("{id}/upvote")]
        [Authorize]
        public ActionResult Upvote(int id)
        {
            return Vote(id, Models.Vote.Upvote);
        }

        [HttpPut]
        [Route("{id}/downvote")]
        [Authorize]
        public ActionResult Downvote(int id)
        {
            return Vote(id, Models.Vote.Downvote);
        }

        [HttpPut]
        [Route("{id}/nullvote")]
        [Authorize]
        public ActionResult NullVote(int id)
        {
            return Vote(id, Models.Vote.None);
        }

        private ActionResult Vote(int id, Vote vote)
        {
            var problem = Context.Problems.First(p => p.Id == id);
            if (vote == Models.Vote.Upvote) problem.Points++;
            if (vote == Models.Vote.Downvote) problem.Points--;
            Context.Problems.Update(problem);
            Context.SaveChanges();
            return StatusCode(200, new {points = problem.Points});

            //var author = AuthorsController.GetAuthor(HttpContext.User, Context);
            //var containsKey = author.VotedProblems.Any(voted => voted.ElementId == problem.Id);
            //if (containsKey)
            //{
            //    var element = author.VotedProblems.First(e => e.ElementId == problem.Id);
            //    if (vote == Models.Vote.None) author.VotedProblems.Remove(element);
            //    else if (vote == element.Vote) return StatusCode(200, new {success = false});
            //    element.Vote = vote;
            //    Context.Authors.Update(author);
            //    Context.SaveChanges();
            //    return StatusCode(200, new {success = true});
            //}

            //if (vote == Models.Vote.None) return StatusCode(200, new {success = false});
            //author.VotedProblems.Add(new VoteElement {ElementId = problem.Id, Vote = vote});
            //Context.Authors.Update(author);
            //Context.SaveChanges();

            //return StatusCode(200, new {success = true});
        }
    }
}