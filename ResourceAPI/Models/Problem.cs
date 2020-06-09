using System.Collections.Generic;

namespace ResourceAPI.Models
{
    public class Problem : Post
    {
        //public int Id { get; set; }
        public string Title { get; set; }

        //public string Content { get; set; }
        public List<Answer> Answers { get; set; } = new List<Answer>();

        public ICollection<Comment> Comments { get; set; }
        //public int Points { get; set; }
    }
}