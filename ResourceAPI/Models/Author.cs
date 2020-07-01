using System.Collections.Generic;
using System.Linq;

namespace ResourceAPI.Models
{
    public enum Vote
    {
        None,
        Downvote,
        Upvote
    }

    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public List<Problem> Problems { get; set; } = new List<Problem>();
        public List<Answer> Answers { get; set; } = new List<Answer>();

        public List<ProblemVote> ProblemVotes { get; set; } = new List<ProblemVote>();
        public List<AnswerVote> AnswerVotes { get; set; } = new List<AnswerVote>();

        public List<VoteElement> VotedProblems { get; set; } = new List<VoteElement>();

        public Author NoLists()
        {
            Problems = null;
            Answers = null;
            VotedProblems = null;
            return this;
        }

        public Vote GetVote(SqlContext context, Problem element)
        {
            if (VotedProblems == null) return Vote.None;
            if (VotedProblems.All(vote => vote.ElementId != element.Id)) return Vote.None;
            return VotedProblems.First(vote => vote.ElementId == element.Id).Vote;
        }

        public Author Serializable(int depth = 0)
        {
            if (depth != 0) return this;
            Problems = null;
            Answers = null;
            VotedProblems = null;
            return this;
        }
    }

    public class VoteElement
    {
        public int Id { get; set; }
        public int ElementId { get; set; }
        public Vote Vote { get; set; }
    }
}