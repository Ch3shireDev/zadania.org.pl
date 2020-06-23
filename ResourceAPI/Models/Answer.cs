namespace ResourceAPI.Models
{
    public class Answer : Post
    {
        public Problem Parent { get; set; }
        public bool IsApproved { get; set; }
    }
}