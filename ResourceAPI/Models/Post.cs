using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MySql.Data.EntityFrameworkCore.DataAnnotations;

namespace ResourceAPI.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Column(TypeName = "text")]
        [StringLength(1024 * 64)]
        [MySqlCharset("utf8")]
        public string Content { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public int Points { get; set; }
        [NotMapped] public Vote UserVote { get; set; }
        [NotMapped] public bool UserUpvoted { get; set; }
        [NotMapped] public bool UserDownvoted { get; set; }
        public int? AuthorId { get; set; }
        public Author Author { get; set; }
        public DateTime Created { get; set; }
        public DateTime Edited { get; set; }
    }
}