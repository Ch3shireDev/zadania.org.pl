using System.Linq;
using Microsoft.EntityFrameworkCore;
using ResourceAPI;
using ResourceAPI.ApiServices;
using ResourceAPI.Models.Category;
using Xunit;

namespace ResourceAPITests
{
    public class CategoryServiceTests
    {
        public CategoryServiceTests()
        {
            _optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("zadania");
            var context = new SqlContext(_optionsBuilder.Options);
            _categoryService = new CategoryService(context);
        }

        private readonly ICategoryService _categoryService;
        private readonly DbContextOptionsBuilder _optionsBuilder;

        [Fact]
        public void BrowseCategoryTest()
        {
            var id = _categoryService.Create(1, new Category {Name = "abc"});
            _categoryService.Create(id, new Category {Name = "cde"});
            _categoryService.Create(id, new Category {Name = "xyz"});
            _categoryService.Create(id, new Category {Name = "xyz"});

            var category = _categoryService.Get(id);
            Assert.Equal(3, category.Categories.Count());
        }

        [Fact]
        public void CreateCategoryTest()
        {
            var name = "abc";
            var description = "cde";

            var category = new Category
            {
                Description = description, Name = name
            };

            _categoryService.Create(1, category);
            var root = _categoryService.Get(1);
            var child = _categoryService.Get(2);

            Assert.NotNull(root);
            Assert.NotNull(child);
            Assert.Equal(child.Name, name);
        }
    }
}