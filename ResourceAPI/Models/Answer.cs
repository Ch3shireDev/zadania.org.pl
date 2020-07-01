using System.Collections.Generic;

namespace ResourceAPI.Models
{
    public class Answer : Post
    {
        public Problem Parent { get; set; }
        public int ParentId { get; set; }
        public bool IsApproved { get; set; }

        public ICollection<AnswerVote> AnswerVotes { get; set; }

        public Answer Serializable(int depth = 0)
        {
            if (depth == 0) Parent = null;
            return this;
        }
    }
}