﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceAPI.Models
{
    public class Problem : Post
    {
        [StringLength(64)] public string Title { get; set; }

        [StringLength(64)] public string Source { get; set; }

        public IList<Answer> Answers { get; set; } = new List<Answer>();

        [NotMapped] public IEnumerable<Tag> Tags { get; set; }

        public ICollection<ProblemTag> ProblemTags { get; set; }

        public ICollection<ProblemVote> ProblemVotes { get; set; }
        [NotMapped] public IEnumerable<string> AnswerLinks { get; set; }
        [NotMapped] public bool IsAnswered { get; set; }
        public ICollection<Comment> Comments { get; set; }
        [NotMapped] public string AuthorName { get; set; }

        public new Problem Render()
        {
            ContentHtml = SqlContext.Render(Content, FileData);
            if (Answers != null)
                foreach (var answer in Answers)
                    answer.Render();

            return this;
        }
    }
}