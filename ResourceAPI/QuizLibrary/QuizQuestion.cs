using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CommonLibrary;

namespace QuizLibrary
{
    public class QuizQuestion : Post
    {
        public List<QuizAnswer> Answers { get; set; }
        public Quiz Test { get; set; }

        public int TestId { get; set; }

        //[NotMapped] public IEnumerable<string> AnswerLinks { get; set; }
        public string Solution { get; set; }
        [NotMapped] public string SolutionHtml { get; set; }
        [NotMapped] public string Url => $"/api/v1/quiz/{TestId}/questions/{Id}";

        public new void Render()
        {
            ContentHtml = Tools.Render(Content, FileData);
            SolutionHtml = Tools.Render(Solution, FileData);
        }
    }
}