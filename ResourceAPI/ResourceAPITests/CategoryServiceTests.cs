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
        }

        private readonly DbContextOptionsBuilder _optionsBuilder;
        //private HttpClient _client = new TestClientProvider().Client;
        //private readonly ICategoryService _categoryService;

        [Fact]
        public void CreateCategoryTest()
        {
            var context = new SqlContext(_optionsBuilder.Options);
            var categoryService = new CategoryService(context);

            var name = "abc";
            var description = "cde";

            var category = new Category
            {
                Description = description, Name = name, Url = "abc"
            };

            categoryService.Create(1, category);
            var root = categoryService.Get(1);
            var child = categoryService.Get(2);

            Assert.NotNull(root);
            Assert.NotNull(child);
            Assert.Equal(child.Name, name);
            Assert.Equal(child.Description, description);
        }

        [Fact]
        public void BrowseCategoryTest()
        {
            var context = new SqlContext(_optionsBuilder.Options);
            var categoryService = new CategoryService(context);
            categoryService.Create(1, new Category {Name = "abc"});
            categoryService.Create(1, new Category {Name = "cde"});
            categoryService.Create(1, new Category {Name = "xyz"});

            var category = categoryService.Get(1);
            Assert.Equal(3, category.ChildCategories.Count());
            //Assert.Equal("/api/v1/");

        }
    }
}