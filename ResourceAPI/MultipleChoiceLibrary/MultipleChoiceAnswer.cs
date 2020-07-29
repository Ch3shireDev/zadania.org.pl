using System.ComponentModel.DataAnnotations.Schema;
using CommonLibrary;

namespace MultipleChoiceLibrary
{
    public class MultipleChoiceAnswer : Post
    {
        public bool IsCorrect { get; set; }
        public MultipleChoiceQuestion Question { get; set; }
        public int QuestionId { get; set; }
        public int TestId { get; set; }
        [NotMapped] public string Url => $"/api/v1/multiple-choice-tests/{TestId}/questions/{QuestionId}/answers/{Id}";
    }
}