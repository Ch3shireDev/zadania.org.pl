using ResourceAPI.Enums;
using ResourceAPI.Models.Post;

namespace ResourceAPI.Models.Problem
{
    public class ProblemVote
    {
        public Author Author { get; set; }
        public int AuthorId { get; set; }
        public Problem Problem { get; set; }
        public int ProblemId { get; set; }
        public Vote Vote { get; set; }
    }
}