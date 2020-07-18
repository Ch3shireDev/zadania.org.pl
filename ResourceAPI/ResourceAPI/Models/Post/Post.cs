using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ResourceAPI.Enums;

namespace ResourceAPI.Models.Post
{
    public class Post
    {
        public int Id { get; set; }

        [StringLength(8000)]
        [Column("Content")]
        public string Content { get; set; }

        [NotMapped] public string ContentHtml { get; set; }
        public int Points { get; set; }
        [NotMapped] public Vote UserVote { get; set; }
        [NotMapped] public bool UserUpvoted { get; set; }
        [NotMapped] public bool UserDownvoted { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public DateTime Created { get; set; }
        public DateTime Edited { get; set; }

        public ICollection<FileData> FileData { get; set; }

        public void Render()
        {
            ContentHtml = Tools.Tools.Render(Content, FileData);
        }
    }
}