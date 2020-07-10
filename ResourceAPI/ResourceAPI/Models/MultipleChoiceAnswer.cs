namespace ResourceAPI.Models
{
    public class MultipleChoiceAnswer : Post
    {
        public bool IsCorrectAnswer { get; set; }
        public MultipleChoiceQuestion Question { get; set; }
        public int QuestionId { get; set; }
    }
}