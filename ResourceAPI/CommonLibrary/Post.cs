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

        //[NotMapped] public string ContentHtml { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public DateTime Created { get; set; }
        public DateTime Edited { get; set; }
        public ICollection<FileData> FileData { get; set; } = new List<FileData>();
        [NotMapped] public IEnumerable<FileDataView> Files { get; set; } = new List<FileDataView>();

        public int CategoryId { get; set; }

        public void Render()
        {
            Content = Tools.Render(Content, Files);
        }
    }
}