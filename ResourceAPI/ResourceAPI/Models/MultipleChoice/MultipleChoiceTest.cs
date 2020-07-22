using System.Collections.Generic;

namespace ResourceAPI.Models.MultipleChoice
{
    public class MultipleChoiceTest : Post.Post
    {
        public string Name { get; set; }
        public List<MultipleChoiceQuestion> Questions { get; set; }
        public bool CanBeRandomized { get; set; }
    }
}