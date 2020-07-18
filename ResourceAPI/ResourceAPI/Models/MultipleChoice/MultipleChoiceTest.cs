﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceAPI.Models.MultipleChoice
{
    public class MultipleChoiceTest : Post.Post
    {
        public string Title { get; set; }
        public List<MultipleChoiceQuestion> Questions { get; set; }
        public bool CanBeRandomized { get; set; }
        [NotMapped] public string AuthorLink { get; set; }
        [NotMapped] public IEnumerable<string> QuestionLinks { get; set; }
    }
}