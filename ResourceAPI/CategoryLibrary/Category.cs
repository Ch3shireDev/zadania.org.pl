using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CommonLibrary;
using ExerciseLibrary;
using FileDataLibrary;
using ProblemLibrary;
using QuizLibrary;

namespace CategoryLibrary
{
    public class Category
    {
        public int Id { get; set; }

        [NotMapped] public ICollection<FileDataView> FileData { get; set; } = new List<FileDataView>();

        public string Name { get; set; }
        public string Description { get; set; }
        public Category Parent { get; set; }
        public int? ParentId { get; set; }
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Problem> Problems { get; set; } = new List<Problem>();
        public IEnumerable<Exercise> Exercises { get; set; } = new List<Exercise>();
        public IEnumerable<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public int AuthorId { get; set; }

        public CategoryView ToView()
        {
            var categoryView = new CategoryView
            {
                Id = Id,
                Name = Name,
                ParentId = ParentId,
                Description = Tools.Render(Description, FileData),
                CategoriesCount = Categories.Count(),
                Categories = Categories.Select(c => c.ToLink()).ToArray(),
                ProblemsCount = Problems.Count(),
                ExercisesCount = Exercises.Count(),
                QuizzesCount = Quizzes.Count()
            };
            return categoryView;
        }

        public CategoryLink ToLink()
        {
            return new CategoryLink {Id = Id, Name = Name};
        }
    }
}