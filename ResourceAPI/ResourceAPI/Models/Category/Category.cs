using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ResourceAPI.Models.MultipleChoice;

namespace ResourceAPI.Models.Category
{
    public class Category
    {
        public int Id { get; set; }
        [NotMapped] public string Url => $"/api/v1/categories/{Id}";
        public string Name { get; set; }
        public string Description { get; set; }
        [NotMapped] public string DescriptionHtml { get; set; }
        public Category Parent { get; set; }
        public int? ParentId { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Problem.Problem> Problems { get; set; }
        public IEnumerable<Exercise.Exercise> Exercises { get; set; }
        public IEnumerable<MultipleChoiceTest> MultipleChoiceTests { get; set; }
    }
}