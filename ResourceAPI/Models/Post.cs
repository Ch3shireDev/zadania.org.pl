using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceAPI.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int Points { get; set; }
        [NotMapped] public Vote UserVote { get; set; }
        public int? AuthorId { get; set; }
        public Author Author { get; set; }
        public DateTime Created { get; set; }
        public DateTime Edited { get; set; }
    }
}