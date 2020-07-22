using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using ResourceAPI;
using ResourceAPI.ApiServices;
using ResourceAPI.ApiServices.Interfaces;
using ResourceAPI.Models.Category;
using ResourceAPI.Models.MultipleChoice;
using ResourceAPI.Models.Problem;
using Xunit;

namespace ResourceAPITests.ServicesTests
{
    public class CategoryServiceTests
    {
        public CategoryServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("zadania");
            _context = new SqlContext(optionsBuilder.Options);
            _categoryService = new CategoryService(_context);
            _problemService = new ProblemService(_context, _categoryService);
            _multipleChoiceService = new MultipleChoiceService(_context, _categoryService);
            _authorService = new AuthorService(_context);
        }

        private readonly SqlContext _context;
        private readonly ICategoryService _categoryService;
        private readonly IProblemService _problemService;
        private readonly IAuthorService _authorService;
        private readonly IMultipleChoiceService _multipleChoiceService;

        [Fact]
        public void AddMultipleChoiceTest()
        {
            Thread.Sleep(500);
            var id = _categoryService.Create(1, new Category {Name = "abc"});
            _categoryService.Create(id, new Category {Name = "cde"});
            _categoryService.Create(id, new Category {Name = "xyz"});
            var id2 = _categoryService.Create(id, new Category {Name = "xyz"});

            var pid0 = _multipleChoiceService.CreateTest(id2, new MultipleChoiceTest {Name = "xxx1"});
            var pid1 = _multipleChoiceService.CreateTest(id2, new MultipleChoiceTest {Name = "xxx2"});
            var pid2 = _multipleChoiceService.CreateTest(id2, new MultipleChoiceTest {Name = "xxx3"});

            var category = _categoryService.Get(id2);

            Assert.Equal("xyz", category.Name);
            Assert.Equal(3, category.MultipleChoiceTests.Count());
            var tests = category.MultipleChoiceTests.ToList();
            Assert.Contains(tests, p => p.Id == pid0);
            Assert.Contains(tests, p => p.Id == pid1);
            Assert.Contains(tests, p => p.Id == pid2);
        }

        [Fact]
        public void AddProblemsTest()
        {
            var id = _categoryService.Create(1, new Category {Name = "abc"});
            _categoryService.Create(id, new Category {Name = "cde"});
            _categoryService.Create(id, new Category {Name = "xyz"});
            var id2 = _categoryService.Create(id, new Category {Name = "xyz"});

            var author = _authorService.GetAuthor(1);
            var pid0 = _problemService.Create(id2, new Problem {Name = "xxx1"});
            var pid1 = _problemService.Create(id2, new Problem {Name = "xxx2"});
            var pid2 = _problemService.Create(id2, new Problem {Name = "xxx3"});

            var category = _categoryService.Get(id2);

            Assert.Equal("xyz", category.Name);
            Assert.Equal(3, category.Problems.Count());
            var problems = category.Problems.ToList();
            Assert.Contains(problems, p => p.Id == pid0);
            Assert.Contains(problems, p => p.Id == pid1);
            Assert.Contains(problems, p => p.Id == pid2);
        }

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
            var categoryId = _categoryService.Create(1, new Category {Name = "abc"});

            var initial = _categoryService.Get(categoryId).Problems.Count();

            _problemService.Create(categoryId, new Problem {Name = "xyz"});
            _problemService.Create(categoryId, new Problem {Name = "xyz"});
            _problemService.Create(categoryId, new Problem {Name = "xyz"});

            Assert.Equal("abc", _context.Categories.First(c => c.Id == categoryId).Name);
            Assert.Equal(initial + 3, _categoryService.Get(categoryId).Problems.Count());

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

            _problemService.Create(categoryId, new Problem {Name = "xyz"});
            _problemService.Create(categoryId, new Problem {Name = "xyz"});
            _problemService.Create(categoryId, new Problem {Name = "xyz"});

            Assert.Equal("abc", _context.Categories.First(c => c.Id == categoryId).Name);
            Assert.Equal(3, _categoryService.Get(categoryId).Problems.Count());

            _categoryService.Update(categoryId, new Category {Name = "xxx"});
            Assert.Equal("xxx", _context.Categories.First(c => c.Id == categoryId).Name);
            Assert.Equal(3, _categoryService.Get(categoryId).Problems.Count());
        }
    }
}