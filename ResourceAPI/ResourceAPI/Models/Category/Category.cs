using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceAPI.Models.Category
{
    public class Category
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [NotMapped] public string DescriptionHtml { get; set; }
        public Category Parent { get; set; }
        public int ParentId { get; set; }
        public IEnumerable<Category> ChildCategories { get; set; }
        public IEnumerable<Post.Post> ChildPosts { get; set; }
    }
}