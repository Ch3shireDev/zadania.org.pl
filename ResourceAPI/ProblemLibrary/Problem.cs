using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CommonLibrary;

namespace ProblemLibrary
{
    /// <summary>
    ///     Schemat problemu zamieszczanego przez użytkownika.
    /// </summary>
    public class Problem : Post
    {
        [StringLength(64)] public string Name { get; set; }

        public IList<Answer> Answers { get; set; } = new List<Answer>();

        [NotMapped] public IEnumerable<Tag> Tags { get; set; }

        //public ICollection<ProblemTag> ProblemTags { get; set; }

        //public ICollection<ProblemVote> ProblemVotes { get; set; }

        [NotMapped] public bool IsSolved { get; set; }
        public ICollection<Comment> Comments { get; set; }
        [NotMapped] public string AuthorName { get; set; }

        public new Problem Render()
        {
            ContentHtml = Tools.Render(Content, FileData);
            Content = null;
            FileData = null;
            return this;
        }

        public ProblemLink ToLink()
        {
            return new ProblemLink
            {
                Id = Id,
                Name = Name
            };
        }

        public ProblemView ToView()
        {
            return new ProblemView
            {
                Id = Id,
                Name = Name,
                Content = Tools.Render(Content, FileData),
                IsSolved = IsSolved,
                Answers = Answers.Select(a => new AnswerView
                {
                    Id = a.Id,
                    ProblemId = a.ProblemId,
                    IsApproved = a.IsApproved,
                    Content = Tools.Render(a.Content, a.FileData)
                })
            };
        }
    }
}