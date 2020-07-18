using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ResourceAPI.Models.Problem;

namespace ResourceAPI.Models.Post
{
    public class Author
    {
        public int Id { get; set; }

        [StringLength(128)] public string Name { get; set; }

        [StringLength(64)] public string Email { get; set; }

        [StringLength(64)] public string UserId { get; set; }

        public List<Problem.Problem> Problems { get; set; } = new List<Problem.Problem>();
        public List<Answer> Answers { get; set; } = new List<Answer>();
        public List<ProblemVote> ProblemVotes { get; set; } = new List<ProblemVote>();
        public List<AnswerVote> AnswerVotes { get; set; } = new List<AnswerVote>();
        public List<VoteElement> VotedProblems { get; set; } = new List<VoteElement>();
    }
}