using System.ComponentModel.DataAnnotations.Schema;
using CommonLibrary;

namespace QuizLibrary
{
    public class QuizAnswer : Post
    {
        public bool IsCorrect { get; set; }
        public QuizQuestion Question { get; set; }
        public int QuestionId { get; set; }
        public int TestId { get; set; }
        [NotMapped] public string Url => $"/api/v1/quiz/{TestId}/questions/{QuestionId}/answers/{Id}";
    }
}