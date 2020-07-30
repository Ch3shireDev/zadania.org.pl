using System.Collections.Generic;
using CommonLibrary;

namespace QuizLibrary
{
    public class Quiz : Post
    {
        public string Name { get; set; }
        public List<QuizQuestion> Questions { get; set; }
        public bool CanBeRandomized { get; set; }
    }
}