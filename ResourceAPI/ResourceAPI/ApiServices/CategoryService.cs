using System.Collections.Generic;
using System.Linq;
using ResourceAPI.Models.Category;

namespace ResourceAPI.ApiServices
{
    public interface ICategoryService
    {
        IEnumerable<Category> Browse();
        Category Get(int id);
        bool Create(int id, Category category);
        bool Update(int id, Category category);
        bool Delete(int id);
        bool SetParent(int parentId, int childId);
    }

    public class CategoryService : ICategoryService
    {
        private readonly SqlContext _context;

        public CategoryService(SqlContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> Browse()
        {
            return _context.Categories.ToList();
        }

        public Category Get(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == id);
        }

        public bool Create(int id, Category category)
        {
            var rootCategory = Get(id);
            if (rootCategory == null) return false;
            var newCategory = new Category
            {
                Description = category.Description,
                Url = category.Url,
                Name = category.Name,
                ParentId = id
            };
            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            return true;
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

        public bool SetParent(int parentId, int childId)
        {
            if (!Exists(parentId)) return false;
            var child = Get(childId);
            if (child == null) return false;
            child.ParentId = parentId;
            _context.Categories.Update(child);
            _context.SaveChanges();
            return true;
        }

        public bool Exists(int id)
        {
            return _context.Categories.Any(c => c.Id == id);
        }
    }
}