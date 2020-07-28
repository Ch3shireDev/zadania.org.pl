using System.Collections.Generic;
using CommonLibrary;

namespace MultipleChoiceLibrary
{
    public class MultipleChoiceTest : Post
    {
        public string Name { get; set; }
        public List<MultipleChoiceQuestion> Questions { get; set; }
        public bool CanBeRandomized { get; set; }
    }
}