namespace VoteLibrary
{
    public class VoteService : IVoteService
    {
        public VoteService(IVoteDbContext context)
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
}