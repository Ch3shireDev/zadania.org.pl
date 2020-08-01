using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CommonLibrary;
using ExerciseLibrary;
using ProblemLibrary;
using QuizLibrary;

namespace CategoryLibrary
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

        public IEnumerable<Exercise> Exercises { get; set; } = new List<Exercise>();

        public IEnumerable<Quiz> QuizTests { get; set; } = new List<Quiz>();
        public int AuthorId { get; set; }

        public Category Render()
        {
            DescriptionHtml = Tools.Render(Description, FileData);
            FileData = null;
            return this;
        }

        public CategoryView AsView()
        {
            var categoryView = new CategoryView
            {
                Id = Id,
                Name = Name,
                Description = Tools.Render(Description, FileData),
                Url = Url,
                ParentUrl = $"/api/v1/categories/{ParentId}",
                Categories = Categories.Select(c => c.AsLink()).ToArray(),
                Problems = Problems.Select(p => p.AsLink()).ToArray()
            };
            return categoryView;
        }

        public CategoryLink AsLink()
        {
            return new CategoryLink {Id = Id, Name = Name, Url = Url};
        }
    }
}