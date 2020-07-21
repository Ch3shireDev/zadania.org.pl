using System.Linq;
using Microsoft.EntityFrameworkCore;
using ResourceAPI;
using ResourceAPI.ApiServices;
using ResourceAPI.ApiServices.Interfaces;
using ResourceAPI.Models.Category;
using ResourceAPI.Models.Problem;
using Xunit;

namespace ResourceAPITests
{
    public class CategoryServiceTests
    {
        public CategoryServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("zadania");
            _context = new SqlContext(optionsBuilder.Options);
            _categoryService = new CategoryService(_context);
            _problemService = new ProblemService(_context, _categoryService);
            _authorService = new AuthorService(_context);
        }

        private readonly SqlContext _context;
        private readonly ICategoryService _categoryService;
        private readonly IProblemService _problemService;
        private readonly IAuthorService _authorService;

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
        public void CreateCategoryAndFillWithProblems()
        {
            var id = _categoryService.Create(1, new Category {Name = "abc"});
            _categoryService.Create(id, new Category {Name = "cde"});
            _categoryService.Create(id, new Category {Name = "xyz"});
            var id2 = _categoryService.Create(id, new Category {Name = "xyz"});

            var author = _authorService.GetAuthor(1);
            var pid0 = _problemService.Create(id2, new Problem {Title = "xxx1"}, author);
            var pid1 = _problemService.Create(id2, new Problem {Title = "xxx2"}, author);
            var pid2 = _problemService.Create(id2, new Problem {Title = "xxx3"}, author);

            var category = _categoryService.Get(id2);

            Assert.Equal("xyz", category.Name);
            Assert.Equal(3, category.Problems.Count());
            var problems = category.Problems.ToList();
            Assert.Contains(problems, p => p.Id == pid0);
            Assert.Contains(problems, p => p.Id == pid1);
            Assert.Contains(problems, p => p.Id == pid2);
        }

        [Fact]
        public void CreateCategoryTest()
        {
            var name = "abc";
            var description = "cde";

            var category = new Category
            {
                Description = description,
                Name = name
            };

            var id = _categoryService.Create(1, category);
            var root = _categoryService.Get(1);
            var child = _categoryService.Get(id);

            Assert.NotNull(root);
            Assert.NotNull(child);
            Assert.Equal(child.Name, name);
        }

        [Fact]
        public void DeleteCategoryTest()
        {
            var author = _authorService.GetAuthor(1);
            var categoryId = _categoryService.Create(1, new Category {Name = "abc"});
            _problemService.Create(categoryId, new Problem {Title = "xyz"}, author);
            _problemService.Create(categoryId, new Problem {Title = "xyz"}, author);
            _problemService.Create(categoryId, new Problem {Title = "xyz"}, author);

            Assert.Equal("abc", _context.Categories.First(c => c.Id == categoryId).Name);
            Assert.Equal(3, _categoryService.Get(categoryId).Problems.Count());

            var categoriesNum = _context.Categories.Count();
            var problemsNum = _context.Problems.Count();

            var result = _categoryService.Delete(categoryId);
            Assert.True(result);

            var categoriesNumAfter = _context.Categories.Count();
            var problemsNumAfter = _context.Problems.Count();

            Assert.Equal(categoriesNum - 1, categoriesNumAfter);
            Assert.Equal(problemsNum - 3, problemsNumAfter);
        }

        [Fact]
        public void EditCategoryTest()
        {
            var author = _authorService.GetAuthor(1);
            var categoryId = _categoryService.Create(1, new Category {Name = "abc"});

            _problemService.Create(categoryId, new Problem {Title = "xyz"}, author);
            _problemService.Create(categoryId, new Problem {Title = "xyz"}, author);
            _problemService.Create(categoryId, new Problem {Title = "xyz"}, author);

            Assert.Equal("abc", _context.Categories.First(c => c.Id == categoryId).Name);
            Assert.Equal(3, _categoryService.Get(categoryId).Problems.Count());

            _categoryService.Update(categoryId, new Category {Name = "xxx"});
            Assert.Equal("xxx", _context.Categories.First(c => c.Id == categoryId).Name);
            Assert.Equal(3, _categoryService.Get(categoryId).Problems.Count());
        }
    }
}