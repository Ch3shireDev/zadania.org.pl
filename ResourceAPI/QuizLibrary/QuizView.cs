using System.Collections.Generic;

namespace QuizLibrary
{
    public class QuizView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }

        public IEnumerable<QuizQuestionLink> Questions { get; set; }
    }
}