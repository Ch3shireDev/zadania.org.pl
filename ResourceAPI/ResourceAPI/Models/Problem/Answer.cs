using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ResourceAPI.Models.Post;

namespace ResourceAPI.Models.Problem
{
    public class Answer : Post.Post
    {
        public Problem Problem { get; set; }
        [NotMapped] public string Url => $"/api/v1/problems/{ProblemId}/answers/{Id}";
        public int ProblemId { get; set; }
        public bool IsApproved { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public ICollection<AnswerVote> AnswerVotes { get; set; }
        [NotMapped] public string AuthorName { get; set; }
        [NotMapped] public string UserId { get; set; }

        public Answer Serializable(int depth = 0)
        {
            if (depth == 0) Problem = null;
            return this;
        }

        public new Answer Render()
        {
            ContentHtml = Tools.Tools.Render(Content, FileData);
            return this;
        }
    }
}