using System.Collections.Generic;

namespace ResourceAPI.Models
{
    public class MultipleChoiceQuestion : Post
    {
        public List<MultipleChoiceAnswer> Answers { get; set; }
    }

    public class MultipleChoiceAnswer
    {
        public string Content { get; set; }
    }
}