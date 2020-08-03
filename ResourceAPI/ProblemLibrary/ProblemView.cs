using System.Collections.Generic;

namespace ProblemLibrary
{
    public class ProblemView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public bool IsSolved { get; set; }
        public int Points { get; set; }
        public int UserVote { get; set; }
        public IEnumerable<AnswerView> Answers { get; set; }
    }
}