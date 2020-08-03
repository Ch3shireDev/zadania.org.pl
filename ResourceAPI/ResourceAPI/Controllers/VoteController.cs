using CommonLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ResourceAPI.Controllers
{
    public interface IVoteDbInterface
    {
        public DbSet<VoteElement> Votes { get; set; }
    }

    public interface IVoteService
    {
        void VoteProblem(int id, int dir);
    }

    public class VoteService : IVoteService
    {
        public VoteService(IVoteDbInterface context)
        {
        }

        public void VoteProblem(int id, int dir)
        {
        }


        public void VoteProblem(int problemId, int dir, int authorId)
        {
            //var problemVote =
            //    _context.ProblemVotes.FirstOrDefault(pv => pv.AuthorId == AuthorId && pv.ProblemId == problemId);
            //if (problemVote == null)
            //{
            //    problemVote = new ProblemVote { AuthorId = AuthorId, ProblemId = problemId };
            //    _context.ProblemVotes.Add(problemVote);
            //}
            //else
            //{
            //    problemVote.Vote = problemVote.Vote == vote ? Vote.None : vote;
            //    _context.ProblemVotes.Update(problemVote);
            //}

            //_context.SaveChanges();

            //var problem = Get(problemId);

            //problem.Points = _context.ProblemVotes.Where(pv => pv.ProblemId == problem.Id)
            //    .Select(pv => pv.Vote == Vote.Upvote ? 1 : pv.Vote == Vote.Downvote ? -1 : 0)
            //    .Sum();

            //_context.Problems.Update(problem);

            //_context.SaveChanges();
        }


        //public IEnumerable<ProblemTag> RefreshTags(Problem problem)
        //{
        //    if (problem.Tags == null) yield break;
        //    foreach (var tag in problem.Tags)
        //    {
        //        tag.Url = tag.GenerateUrl();
        //        var existing = _context.Tags.Find(tag.Url) ?? tag;
        //        yield return new ProblemTag { Problem = problem, Tag = existing, TagUrl = tag.Url };
        //    }
        //}
    }

    [ApiController]
    [Route("/api/v1/vote")]
    public class VoteController : Controller
    {
        private IVoteService _voteService;

        public VoteController(IVoteService voteService)
        {
            _voteService = voteService;
        }
        //[HttpGet("{id}/points")]
        //public ActionResult Points(int id)
        //{
        //    var points = _context.Problems.First(problem => problem.Id == id).Points;
        //    return StatusCode(200, new {points});
        //}


        [HttpPost("{id}/upvote")]
        [Authorize]
        public ActionResult UpvoteProblem(int id)
        {
            //_problemService.VoteProblem(id, Vote.Upvote);
            return Ok();
        }

        [HttpPost("{id}/downvote")]
        [Authorize]
        public ActionResult DownvoteProblem(int id)
        {
            //_problemService.VoteProblem(id, Vote.Downvote);
            return Ok();
        }
    }
}