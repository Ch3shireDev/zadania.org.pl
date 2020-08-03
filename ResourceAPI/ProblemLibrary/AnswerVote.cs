using CommonLibrary;

namespace ProblemLibrary
{
    public class AnswerVote
    {
        public Answer Answer { get; set; }
        public int AnswerId { get; set; }
        public Author Author { get; set; }

        public int AuthorId { get; set; }
        //public Vote Vote { get; set; }
    }
}