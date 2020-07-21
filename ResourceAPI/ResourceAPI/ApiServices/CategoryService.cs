using System.Collections.Generic;
using System.Linq;
using ResourceAPI.Models.Category;

namespace ResourceAPI.ApiServices
{
    public interface ICategoryService
    {
        IEnumerable<Category> Browse();
        Category Get(int id);
        int Create(int id, Category category);
        bool Update(int id, Category category);

        bool Delete(int id);
        //bool SetParent(int parentId, int childId);
    }

    public class CategoryService : ICategoryService
    {
        private readonly SqlContext _context;

        public CategoryService(SqlContext context)
        {
            _context = context;
            EnsureCreated();
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
            var category = _context.Categories
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Categories = c.Categories.Select(cc => new Category {Id = cc.Id, Name = cc.Name}).ToList()
                })
                .FirstOrDefault(c => c.Id == id);

            if (category == null) return category;

            category.Categories = _context.Categories.Where(c => c.ParentId == id)
                .Select(c => new Category {Id = c.Id, Name = c.Name}).ToList();

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
            var baseElement = Get(id);
            if (baseElement == null) return false;
            _context.Entry(baseElement).CurrentValues.SetValues(category);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var category = Get(id);
            if (category == null) return false;
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return true;
        }

        private void EnsureCreated()
        {
            if (_context.Categories.Any(c => c.Id == 1)) return;
            var category = new Category {Name = "Root"};
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public bool Exists(int id)
        {
            return _context.Categories.Any(c => c.Id == id);
        }
    }
}