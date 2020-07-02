using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ResourceAPI.Models
{
    public class Problem : Post
    {
        public string Title { get; set; }
        public string Source { get; set; }
        public IList<Answer> Answers { get; set; } = new List<Answer>();

        [NotMapped] public ICollection<Tag> Tags { get; set; }

        public ICollection<ProblemTag> ProblemTags { get; set; }

        public ICollection<ProblemVote> ProblemVotes { get; set; }

        [NotMapped] public bool IsAnswered { get; set; }
        public Problem Serializable<TResult>(int depth = 0)
        {
            Answers = Answers.Select(a => a.Serializable()).ToArray();
            Tags = ProblemTags?.Select(pc => pc.Tag.Serializable()).ToArray();

            //problem.Tags = problem.ProblemTags.Select(pc => pc.Tag).ToArray();
            if (Author == null) return this;
            Author = Author.Serializable();


            return this;
        }

        public Problem NoLists()
        {
            Answers = null;
            Tags = null;
            ProblemTags = null;
            return this;
        }
    }
}