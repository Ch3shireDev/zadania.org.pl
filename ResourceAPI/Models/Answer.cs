namespace ResourceAPI.Models
{
    public class Answer : Post
    {
        public Problem Parent { get; set; }
    }
}