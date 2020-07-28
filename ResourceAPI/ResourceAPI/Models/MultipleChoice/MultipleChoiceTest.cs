using System.Collections.Generic;
using CommonLibrary;
using ProblemLibrary;

namespace ResourceAPI.Models.MultipleChoice
{
    public class MultipleChoiceTest : Post
    {
        public string Name { get; set; }
        public List<MultipleChoiceQuestion> Questions { get; set; }
        public bool CanBeRandomized { get; set; }
    }
}