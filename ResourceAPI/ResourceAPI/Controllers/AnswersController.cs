﻿using System.Linq;
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
            var answers = Context.Answers.Where(answer => answer.Problem.Id == problemId).Select(a => new
                Answer
                {
                    Id = a.Id,
                    ProblemId = a.ProblemId,
                    Content = a.Content,
                    AuthorId = a.AuthorId,
                    Author = new Author
                        {UserId = a.Author.UserId, Id = a.Author.Id, Name = a.Author.Name, Email = a.Author.Email},
                    //Tags = p.ProblemTags.Select(pt => pt.Tag).ToArray(),
                    //Author = new { p.Author.Name, p.Author.UserId, p.Author.Email, p.Author.Id },
                    Points = a.AnswerVotes.Count(pv => pv.Vote == Models.Vote.Upvote) -
                             a.AnswerVotes.Count(pv => pv.Vote == Models.Vote.Downvote),
                    UserUpvoted = a.AnswerVotes.Any(pv => pv.Vote == Models.Vote.Upvote),
                    UserDownvoted = a.AnswerVotes.Any(pv => pv.Vote == Models.Vote.Downvote)
                }).OrderByDescending(a => a.Points).ToArray().Select(a => a.Render());

            return StatusCode(200, answers.ToArray());
        }

        [HttpGet]
        [Route("{answerId}")]
        public ActionResult Get(int problemId, int answerId)
        {
            var result = Context.Answers.Select(a => new
                Answer
                {
                    Id = a.Id,
                    ProblemId = a.ProblemId,
                    IsApproved = a.IsApproved,
                    Content = a.Content,
                    Points = a.Points,
                    Edited = a.Edited,
                    Created = a.Created,
                    AuthorId = a.AuthorId,
                    Author = new Author
                    {
                        Id = a.Author.Id,
                        Name = a.Author.Name,
                        UserId = a.Author.UserId,
                        Email = a.Author.Email
                    }
                }).FirstOrDefault(a => a.ProblemId == problemId && a.Id == answerId);
            if (result == null) return StatusCode(404);
            result = result.Render();
            return StatusCode(200, result);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Post(int problemId, Answer answer)
        {
            if (!Context.Problems.Any(p => p.Id == problemId)) return StatusCode(404);
            var author = AuthorsController.GetAuthor(HttpContext, Context);
            if (author == null) return StatusCode(403);
            //var userId = HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            //var author = Context.Authors.First(profile => profile.UserId == userId);
            var problem = Context.Problems.First(p => p.Id == problemId);
            answer.Problem = problem;
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

        [NonAction]
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

        [HttpPost]
        [Route("{answerId}/approve")]
        [Authorize]
        public ActionResult ApproveAnswer(int problemId, int answerId)
        {
            var author = AuthorsController.GetAuthor(HttpContext, Context);

            var answer = Context.Answers.FirstOrDefault(a =>
                a.Id == answerId && a.Problem.Id == problemId && a.AuthorId == author.Id);
            if (answer == null) return StatusCode(403);
            if (answer.IsApproved) return StatusCode(304);
            foreach (var a in Context.Answers.Where(a => a.ProblemId == problemId))
                Context.Entry(a).CurrentValues["IsApproved"] = a.Id == answerId;
            Context.SaveChanges();
            return StatusCode(200);
        }

        [HttpPost]
        [Authorize]
        [Route("{answerId}/disapprove")]
        public ActionResult DisapproveAnswer(int problemId, int answerId)
        {
            var author = AuthorsController.GetAuthor(HttpContext, Context);
            var answer = Context.Answers.FirstOrDefault(a =>
                a.Id == answerId && a.Problem.Id == problemId && a.AuthorId == author.Id);
            if (answer == null) return StatusCode(403);
            if (!answer.IsApproved) return StatusCode(304);
            Context.Entry(answer).CurrentValues["IsApproved"] = false;

            Context.SaveChanges();
            return StatusCode(200);
        }


        [HttpPost]
        [Route("{answerId}/upvote")]
        [Authorize]
        public ActionResult UpvoteAnswer(int id)
        {
            return VoteAnswer(id, Models.Vote.Upvote);
        }

        [HttpPost]
        [Route("{answerId}/downvote")]
        [Authorize]
        public ActionResult DownvoteAnswer(int id)
        {
            return VoteAnswer(id, Models.Vote.Downvote);
        }

        public ActionResult VoteAnswer(int id, Vote vote)
        {
            var answer = Context.Answers.FirstOrDefault(p => p.Id == id);
            if (answer == null) return StatusCode(403);
            var author = AuthorsController.GetAuthor(HttpContext, Context);
            if (author == null) return StatusCode(403);
            var answerVote = Context.AnswerVotes.FirstOrDefault(pv => pv.AuthorId == author.Id && pv.AnswerId == id);
            if (answerVote == null)
            {
                answerVote = new AnswerVote {Author = author, Answer = answer};
                Context.AnswerVotes.Add(answerVote);
            }
            else
            {
                answerVote.Vote = answerVote.Vote == vote ? Models.Vote.None : vote;
                Context.AnswerVotes.Update(answerVote);
            }

            Context.SaveChanges();

            answer.Points = Context.AnswerVotes
                .Where(av => av.AnswerId == answer.Id)
                .Select(av => av.Vote == Models.Vote.Upvote ? 1 : av.Vote == Models.Vote.Downvote ? -1 : 0)
                .Sum();

            Context.Answers.Update(answer);

            Context.SaveChanges();

            return StatusCode(200, answerVote.Vote);
        }
    }
}