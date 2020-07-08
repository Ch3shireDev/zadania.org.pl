using System.Collections.Generic;

namespace ResourceAPI.Models
{
    public class MultipleChoiceTest : Post
    {
        public List<MultipleChoiceQuestion> Questions { get; set; }
        public bool CanBeRandomized { get; set; }
    }
}