using System.Collections.Generic;
using System.Linq;
using ResourceAPI.ApiServices.Interfaces;
using ResourceAPI.Models.Category;
using ResourceAPI.Models.Problem;

namespace ResourceAPI.ApiServices
{
    public class CategoryService : ICategoryService
    {
        private readonly SqlContext _context;

        public CategoryService(SqlContext context)
        {
            _context = context;
            if (_context.Categories.Any(c => c.Id == 1)) return;
            var category = new Category {Name = "Root"};
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public IEnumerable<Category> Browse()
        {
            return _context.Categories
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Categories = c.Categories.Select(cc => new Category {Id = cc.Id, Name = cc.Name}).ToList()
                })
                .ToList();
        }

        public Category Get(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var category = _context.Categories
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Categories = c.Categories.Select(cc => new Category {Id = cc.Id, Name = cc.Name}).ToList(),
                    Problems = c.Problems.Select(cp => new Problem {Id = cp.Id, Title = cp.Title}).ToList()
                })
                .FirstOrDefault(c => c.Id == id);

            if (category == null) return null;

            category.Categories = _context.Categories.Where(c => c.ParentId == id)
                .Select(c => new Category {Id = c.Id, Name = c.Name}).ToList();
            //var problems = _context.Problems.ToList();
            return category;
        }

        public int Create(int id, Category category)
        {
            var rootCategory = Get(id);
            if (rootCategory == null) return 0;
            var newCategory = new Category
            {
                Description = category.Description,
                Name = category.Name,
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