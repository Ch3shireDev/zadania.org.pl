using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FileDataLibrary;

namespace CommonLibrary
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        [NotMapped] public string ContentHtml { get; set; }

        public int Points { get; set; }

        //[NotMapped] public Vote UserVote { get; set; }
        [NotMapped] public bool UserUpvoted { get; set; }
        [NotMapped] public bool UserDownvoted { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public DateTime Created { get; set; }
        public DateTime Edited { get; set; }
        public ICollection<FileData> FileData { get; set; } = new List<FileData>();
        [NotMapped] public IEnumerable<FileDataView> Files { get; set; } = new List<FileDataView>();

        //public Category.Category Category { get; set; }
        public int CategoryId { get; set; }

        public void Render()
        {
            ContentHtml = Tools.Render(Content, Files);
        }
    }
}