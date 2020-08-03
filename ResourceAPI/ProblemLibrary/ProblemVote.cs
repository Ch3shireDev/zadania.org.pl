using CommonLibrary;

namespace ProblemLibrary
{
    public class ProblemVote
    {
        public Author Author { get; set; }
        public int AuthorId { get; set; }
        public Problem Problem { get; set; }

        public int ProblemId { get; set; }
        //public Vote Vote { get; set; }
    }
}