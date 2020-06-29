using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceAPI.Models;

namespace ResourceAPI.Controllers
{
    [Route("api/v1/problems/{problemId}/[controller]")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly ILogger<ProblemsController> _logger;

        public AnswersController(ILogger<ProblemsController> logger, SqlContext context)
        {
            _logger = logger;
            Context = context;
        }

        private SqlContext Context { get; }

        [HttpGet]
        public ActionResult Get(int problemId)
        {
            var answers = Context.Answers.Where(answer => answer.Parent.Id == problemId).ToArray().Reverse().ToList();

            foreach (var answer in answers)
            {
                answer.Parent = null;
                if (answer.Author == null) continue;
                answer.Author.Problems = null;
                answer.Author.Answers = null;
            }

            return StatusCode(200, answers);
        }

        [HttpGet]
        [Route("{answerId}")]
        public ActionResult Get(int problemId, int answerId)
        {
            var result = Context.Answers.First(answer => answer.Id == answerId);
            if (!(result.AuthorId > 0)) return StatusCode(200, result);
            result.Author = Context.Authors.FirstOrDefault(author => author.Id == result.AuthorId);
            if (result.Author == null) return StatusCode(200, result);
            result.Author.Problems = null;
            result.Author.Answers = null;
            return StatusCode(200, result);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Post(int problemId, Answer answer)
        {
            if (!Context.Problems.Any(p => p.Id == problemId)) return StatusCode(404);
            var userId = HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var author = Context.Authors.First(profile => profile.UserId == userId);
            var problem = Context.Problems.First(p => p.Id == problemId);
            answer.Parent = problem;
            answer.Author = author;
            problem.Answers.Add(answer);
            Context.Problems.Update(problem);
            Context.SaveChanges();
            return StatusCode(201);
        }

        [HttpPut]
        [Route("{answerId}")]
        [Authorize]
        public ActionResult Put(int problemId, int answerId, Answer answer)
        {
            var originalAnswer = Context.Answers.First(a => a.Id == answerId);
            originalAnswer.Content = answer.Content;
            Context.Answers.Update(originalAnswer);
            Context.SaveChanges();
            return StatusCode(201);
        }

        [HttpDelete]
        [Route("{answerId}")]
        [Authorize]
        public ActionResult Delete(int problemId, int answerId)
        {
            var answer = Context.Answers.First(a => a.Id == answerId);
            Context.Answers.Remove(answer);
            Context.SaveChanges();
            return StatusCode(201);
        }

        [HttpPut]
        [Route("{answerId}/upvote")]
        [Authorize]
        public ActionResult Upvote(int problemId, int answerId)
        {
            return Vote(problemId, answerId, Models.Vote.Upvote);
        }

        [HttpPut]
        [Route("{answerId}/downvote")]
        [Authorize]
        public ActionResult Downvote(int problemId, int answerId)
        {
            return Vote(problemId, answerId, Models.Vote.Downvote);
        }

        public ActionResult Vote(int problemId, int answerId, Vote vote)
        {
            var answer = Context.Answers.First(a => a.Id == answerId);
            if (vote == Models.Vote.Upvote) answer.Points++;
            if (vote == Models.Vote.Downvote) answer.Points--;
            Context.Answers.Update(answer);
            Context.SaveChanges();
            return StatusCode(200, new {success = true, points = answer.Points});
        }

        [HttpGet]
        [Route("{answerId}/points")]
        public ActionResult Points(int problemId, int answerId)
        {
            var answer = Context.Answers.FirstOrDefault(a => a.Id == answerId);
            if (answer == null) return StatusCode(400);
            return StatusCode(200, new {points = answer.Points});
        }
    }
}