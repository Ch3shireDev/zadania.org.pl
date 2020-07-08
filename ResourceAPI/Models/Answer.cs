using System.Collections.Generic;

namespace ResourceAPI.Models
{
    public class Answer : Post
    {
        public Problem Problem { get; set; }
        public int ProblemId { get; set; }
        public bool IsApproved { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public ICollection<AnswerVote> AnswerVotes { get; set; }

        public Answer Serializable(int depth = 0)
        {
            if (depth == 0) Problem = null;
            return this;
        }

        public Answer Render()
        {
            ContentHtml = SqlContext.Render(Content, FileData);
            return this;
        }
    }
}