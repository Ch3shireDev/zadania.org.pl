﻿using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceAPI.Models;
using ResourceAPI.Services;

namespace ResourceAPI.Controllers
{
    [Route("api/v1/problems/{problemId}/[controller]")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly ILogger<ProblemsController> _logger;

        public AnswersController(ILogger<ProblemsController> logger, SqlContext context, IProblemService problemService)
        {
            _logger = logger;
            Context = context;
            ProblemService = problemService;
        }

        private SqlContext Context { get; }
        private IProblemService ProblemService { get; }

        [HttpGet]
        public ActionResult Get(int problemId)
        {
            var answers = Context.Answers
                .Where(answer => answer.Problem.Id == problemId)
                .Select(a => new Answer
                {
                    Id = a.Id,
                    Points = a.Points
                }).OrderByDescending(a => a.Points).ToArray()
                .Select(a => $"/api/v1/problems/{problemId}/answers/{a.Id}");

            return StatusCode(200, answers.ToArray());
        }

        [HttpGet]
        [Route("{answerId}")]
        public ActionResult Get(int problemId, int answerId)
        {
            var answer = ProblemService.GetAnswerById(problemId, answerId);
            if (answer == null) return new NotFoundResult();
            return new OkObjectResult(answer);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Post(int problemId, Answer answer)
        {
            if (!Context.Problems.Any(p => p.Id == problemId)) return StatusCode(404);
            var author = AuthorsController.GetAuthor(HttpContext, Context);
            if (author == null) return StatusCode(403);
            var problem = Context.Problems.First(p => p.Id == problemId);
            answer.Problem = problem;
            answer.ProblemId = problemId;
            answer.Author = author;
            answer.AuthorId = author.Id;
            Context.Answers.Add(answer);
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
            var answer = Context.Answers.FirstOrDefault(a => a.Id == answerId && a.ProblemId == problemId);
            if (answer == null) return StatusCode(404);
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
            var answer = Context.Answers.First(a => a.Id == answerId && a.ProblemId == problemId);
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
            var answer = Context.Answers.FirstOrDefault(a => a.Id == answerId && a.ProblemId == problemId);
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