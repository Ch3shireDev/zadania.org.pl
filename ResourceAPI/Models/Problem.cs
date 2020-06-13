using System.Collections.Generic;

namespace ResourceAPI.Models
{
    public class Problem : Post
    {
        public string Title { get; set; }

        public List<Answer> Answers { get; set; } = new List<Answer>();

        public ICollection<Comment> Comments { get; set; }
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}