using System.Collections.Generic;
using System.Linq;
using ExerciseLibrary;
using Microsoft.EntityFrameworkCore;
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

        public IEnumerable<ProblemLink> GetProblems(int id)
        {
            var category = _context.Categories.Include(c => c.Problems)
                .FirstOrDefault(c => c.Id == id);
            return category?.Problems.Select(p => p.AsLink()).ToArray();
        }

        public IEnumerable<QuizLink> GetQuizzes(int categoryId)
        {
            var category = _context.Categories.Include(c => c.Quizzes).FirstOrDefault(c => c.Id == categoryId);
            return category?.Quizzes.Select(q => q.AsLink()).ToArray();
        }

        public IEnumerable<ExerciseLink> GetExercises(int categoryId)
        {
            var category = _context.Categories.Include(c => c.Exercises).FirstOrDefault(c => c.Id == categoryId);
            return category?.Exercises.Select(e => e.AsLink()).ToArray();
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

        public Category GetCategory(int id)
        {
            var category = _context.Categories.Include(c => c.Categories).FirstOrDefault(c => c.Id == id);
            return category;
        }

        public IEnumerable<CategoryLink> GetCategories(int id)
        {
            var categories = _context.Categories.Select(c => new {c.Id, Categories = c.Categories.ToList()})
                .FirstOrDefault(c => c.Id == id);
            return categories?.Categories.Select(c => c.AsLink());
        }
    }
}