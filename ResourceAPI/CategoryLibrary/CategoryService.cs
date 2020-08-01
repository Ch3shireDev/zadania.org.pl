using System.Linq;
using ProblemLibrary;
using QuizLibrary;

namespace CategoryLibrary
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryDbContext _context;

        public CategoryService(ICategoryDbContext context)
        {
            _context = context;
            if (_context.Categories.Any(c => c.Id == 1)) return;
            var category = new Category {Name = "Root"};
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public Category GetProblems(int id)
        {
            var category = _context.Categories
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    FileData = c.FileData,
                    Description = c.Description,
                    Categories = c.Categories.Select(cc => new Category {Id = cc.Id, Name = cc.Name}).ToList(),
                    Problems = c.Problems.Select(cp => new Problem {Id = cp.Id, Name = cp.Name}).ToList(),
                    QuizTests = c.QuizTests
                        .Select(mt => new Quiz {Id = mt.Id, Name = mt.Name}).ToList()
                })
                .FirstOrDefault(c => c.Id == id);

            if (category == null) return null;

            category.Categories = _context.Categories.Where(c => c.ParentId == id)
                .Select(c => new Category {Id = c.Id, Name = c.Name}).ToList();
            return category.Render();
        }

        public Category GetQuizTests(int categoryId)
        {
            var category = _context.Categories.Select(c => new Category
            {
                Id = c.Id,
                ParentId = c.ParentId,
                QuizTests = c.QuizTests.ToList()
            }).FirstOrDefault(c => c.Id == categoryId);
            return category;
        }

        public Category GetExercises(int categoryId)
        {
            var category = _context.Categories.Select(c => new Category
            {
                Id = c.Id,
                ParentId = c.ParentId,
                Exercises = c.Exercises.ToList()
            }).FirstOrDefault(c => c.Id == categoryId);
            return category;
        }

        public Category Create(Category category, int parentId, int authorId)
        {
            var rootCategory = GetProblems(parentId);
            if (rootCategory == null) return null;
            var newCategory = new Category
            {
                Name = category.Name,
                Description = category.Description,
                ParentId = parentId,
                AuthorId = authorId
            };
            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            return newCategory;
        }

        public Category Update(Category category, int id)
        {
            var baseElement = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (baseElement == null) return null;
            baseElement.Name = category.Name;
            _context.Categories.Update(baseElement);
            _context.SaveChanges();
            return baseElement;
        }

        public bool Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return false;
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return true;
        }
    }
}