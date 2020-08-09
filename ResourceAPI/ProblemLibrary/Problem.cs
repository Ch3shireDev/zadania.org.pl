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

        [NotMapped] public bool IsSolved { get; set; }

        [NotMapped] public string AuthorName { get; set; }

        public new Problem Render()
        {
            Content = Tools.Render(Content, Files);
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
                Content = Tools.Render(Content, Files),
                IsSolved = IsSolved,
                Answers = Answers.Select(a => new AnswerView
                {
                    Id = a.Id,
                    ProblemId = a.ProblemId,
                    IsApproved = a.IsApproved,
                    Content = Tools.Render(a.Content, a.Files)
                })
            };
        }
    }
}