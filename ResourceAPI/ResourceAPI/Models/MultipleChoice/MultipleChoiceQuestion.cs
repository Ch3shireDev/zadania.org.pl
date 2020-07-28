using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CommonLibrary;
using ProblemLibrary;

namespace ResourceAPI.Models.MultipleChoice
{
    public class MultipleChoiceQuestion : Post
    {
        public List<MultipleChoiceAnswer> Answers { get; set; }
        public MultipleChoiceTest Test { get; set; }

        public int TestId { get; set; }

        //[NotMapped] public IEnumerable<string> AnswerLinks { get; set; }
        public string Solution { get; set; }
        [NotMapped] public string SolutionHtml { get; set; }
        [NotMapped] public string Url => $"/api/v1/multiple-choice/{TestId}/questions/{Id}";

        public new void Render()
        {
            ContentHtml = Tools.Render(Content, FileData);
            SolutionHtml = Tools.Render(Solution, FileData);
        }
    }
}