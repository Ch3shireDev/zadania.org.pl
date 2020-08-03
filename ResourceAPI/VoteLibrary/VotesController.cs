using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VoteLibrary
{
    [ApiController]
    [Route("/api/v1/votes")]
    public class VotesController : Controller
    {
        private readonly IVoteService _voteService;

        public VotesController(IVoteService voteService)
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