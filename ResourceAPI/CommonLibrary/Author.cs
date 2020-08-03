using System.ComponentModel.DataAnnotations;

namespace CommonLibrary
{
    public class Author
    {
        public int Id { get; set; }

        [StringLength(128)] public string Name { get; set; }

        [StringLength(64)] public string Email { get; set; }

        [StringLength(64)] public string UserId { get; set; }

        //public List<Problem> Problems { get; set; } = new List<Problem>();
        //public List<Answer> Answers { get; set; } = new List<Answer>();
        //public List<ProblemVote> ProblemVotes { get; set; } = new List<ProblemVote>();
        //public List<AnswerVote> AnswerVotes { get; set; } = new List<AnswerVote>();
        //public List<VoteElement> VotedProblems { get; set; } = new List<VoteElement>();
    }
}