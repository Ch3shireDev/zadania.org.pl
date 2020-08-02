using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CommonLibrary;

namespace ProblemLibrary
{
    public class Answer : Post
    {
        public Problem Problem { get; set; }
        public int ProblemId { get; set; }
        public bool IsApproved { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public ICollection<AnswerVote> AnswerVotes { get; set; }
        [NotMapped] public string AuthorName { get; set; }
        [NotMapped] public string UserId { get; set; }

        public new Answer Render()
        {
            ContentHtml = Tools.Render(Content, FileData);
            return this;
        }

        public AnswerView ToView()
        {
            return new AnswerView
            {
                Id = Id,
                ProblemId = ProblemId,
                IsApproved = IsApproved,
                Content = Tools.Render(Content, FileData)
            };
        }

        public AnswerLink ToLink()
        {
            return new AnswerLink
            {
                Id = Id,
                ProblemId = ProblemId,
                IsApproved = IsApproved
            };
        }
    }
}