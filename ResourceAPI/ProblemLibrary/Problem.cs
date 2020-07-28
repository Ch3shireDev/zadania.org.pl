using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommonLibrary;

namespace ProblemLibrary
{
    public class Problem : Post
    {
        [StringLength(64)] public string Name { get; set; }
        [NotMapped] public string Url => $"/api/v1/problems/{Id}";
        [StringLength(64)] public string Source { get; set; }

        public IList<Answer> Answers { get; set; } = new List<Answer>();

        [NotMapped] public IEnumerable<Tag> Tags { get; set; }

        public ICollection<ProblemTag> ProblemTags { get; set; }

        public ICollection<ProblemVote> ProblemVotes { get; set; }

        //[NotMapped] public IEnumerable<string> AnswerLinks { get; set; }
        [NotMapped] public bool IsSolved { get; set; }
        public ICollection<Comment> Comments { get; set; }
        [NotMapped] public string AuthorName { get; set; }

        public new Problem Render()
        {
            ContentHtml = Tools.Render(Content, FileData);
            FileData = null;
            return this;
        }
    }
}