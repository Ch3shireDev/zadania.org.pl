using System.Linq;
using MultipleChoiceLibrary;
using ProblemLibrary;

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
                    MultipleChoiceTests = c.MultipleChoiceTests
                        .Select(mt => new MultipleChoiceTest {Id = mt.Id, Name = mt.Name}).ToList()
                })
                .FirstOrDefault(c => c.Id == id);

            if (category == null) return null;

            category.Categories = _context.Categories.Where(c => c.ParentId == id)
                .Select(c => new Category {Id = c.Id, Name = c.Name}).ToList();
            return category.Render();
        }

        public Category GetMultipleChoiceTests(int categoryId)
        {
            var category = _context.Categories.Select(c => new Category
            {
                Id = c.Id,
                ParentId = c.ParentId,
                MultipleChoiceTests = c.MultipleChoiceTests.ToList()
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

        public int Create(int id, Category category)
        {
            var rootCategory = GetProblems(id);
            if (rootCategory == null) return 0;
            var newCategory = new Category
            {
                Name = category.Name,
                Description = category.Description,
                ParentId = id
            };
            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            return newCategory.Id;
        }

        public bool Update(int id, Category category)
        {
            var baseElement = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (baseElement == null) return false;
            baseElement.Name = category.Name;
            _context.Categories.Update(baseElement);
            _context.SaveChanges();
            return true;
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