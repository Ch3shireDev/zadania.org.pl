using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CommonLibrary;
using ProblemLibrary;
using ResourceAPI.Models.MultipleChoice;

namespace ResourceAPI.Models.Category
{
    public class Category
    {
        public int Id { get; set; }
        [NotMapped] public string Url => $"/api/v1/categories/{Id}";
        public ICollection<FileData> FileData { get; set; } = new List<FileData>();
        public string Name { get; set; }
        public string Description { get; set; }
        [NotMapped] public string DescriptionHtml { get; set; }
        public Category Parent { get; set; }
        public int? ParentId { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Problem> Problems { get; set; } = new List<Problem>();

        public IEnumerable<Exercise.Exercise> Exercises { get; set; } = new List<Exercise.Exercise>();

        public IEnumerable<MultipleChoiceTest> MultipleChoiceTests { get; set; } = new List<MultipleChoiceTest>();

        public Category Render()
        {
            DescriptionHtml = Tools.Render(Description, FileData);
            FileData = null;
            return this;
        }
    }
}