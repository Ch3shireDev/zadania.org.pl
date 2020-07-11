﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceAPI.Models
{
    public class MultipleChoiceQuestion : Post
    {
        public List<MultipleChoiceAnswer> Answers { get; set; }
        public MultipleChoiceTest Test { get; set; }
        public int TestId { get; set; }
        [NotMapped] public IEnumerable<string> AnswerLinks { get; set; }
        public string Solution { get; set; }
        [NotMapped] public string SolutionHtml { get; set; }

        public new void Render()
        {
            ContentHtml = SqlContext.Render(Content, FileData);
            SolutionHtml = SqlContext.Render(Solution, FileData);
        }
    }
}