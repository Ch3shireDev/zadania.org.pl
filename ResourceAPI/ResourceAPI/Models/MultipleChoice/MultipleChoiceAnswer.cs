using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceAPI.Models.MultipleChoice
{
    public class MultipleChoiceAnswer : Post.Post
    {
        public bool IsCorrect { get; set; }
        public MultipleChoiceQuestion Question { get; set; }
        public int QuestionId { get; set; }
        public int TestId { get; set; }
        [NotMapped] public string Url => $"/api/v1/multiple-choice/{TestId}/questions/{QuestionId}/answers/{Id}";
    }
}