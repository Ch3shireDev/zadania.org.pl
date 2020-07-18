using ResourceAPI.Enums;
using ResourceAPI.Models.Post;

namespace ResourceAPI.Models.Problem
{
    public class AnswerVote
    {
        public Answer Answer { get; set; }
        public int AnswerId { get; set; }
        public Author Author { get; set; }
        public int AuthorId { get; set; }
        public Vote Vote { get; set; }
    }
}