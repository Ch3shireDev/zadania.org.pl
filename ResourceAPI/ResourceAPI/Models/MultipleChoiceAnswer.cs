namespace ResourceAPI.Models
{
    public class MultipleChoiceAnswer : Post
    {
        public bool IsCorrect { get; set; }
        public MultipleChoiceQuestion Question { get; set; }
        public int QuestionId { get; set; }
        public int TestId { get; set; }
    }
}