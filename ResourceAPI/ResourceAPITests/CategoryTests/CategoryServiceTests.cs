using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ResourceAPI;
using ResourceAPI.ApiServices;
using ResourceAPI.ApiServices.Interfaces;
using ResourceAPI.Models.Category;
using ResourceAPI.Models.MultipleChoice;
using ResourceAPI.Models.Problem;
using Xunit;

namespace ResourceAPITests.CategoryTests
{
    public class CategoryServiceTests
    {
        public CategoryServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString());
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
            //Thread.Sleep(500);
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

            var pid0 = _problemService.Create(new Problem {Name = "xxx1", CategoryId = id2});
            var pid1 = _problemService.Create(new Problem {Name = "xxx2", CategoryId = id2});
            var pid2 = _problemService.Create(new Problem {Name = "xxx3", CategoryId = id2});

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
        public void CascadeDeleteTest()
        {
            //Thread.Sleep(1500);

            var first = _categoryService.Create(1, new Category());
            var second = _categoryService.Create(first, new Category());
            var third = _categoryService.Create(second, new Category());
            var fourth = _categoryService.Create(third, new Category());
            var num0 = _context.Categories.Count();
            _categoryService.Delete(first);
            var num1 = _context.Categories.Count();
            Assert.Equal(num0 - 4, num1);
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

            _problemService.Create(new Problem {Name = "xyz", CategoryId = categoryId});
            _problemService.Create(new Problem {Name = "xyz", CategoryId = categoryId});
            _problemService.Create(new Problem {Name = "xyz", CategoryId = categoryId});

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
            var categoryId = _categoryService.Create(1, new Category {Name = "abc"});

            _problemService.Create(new Problem {Name = "xyz", CategoryId = categoryId});
            _problemService.Create(new Problem {Name = "xyz", CategoryId = categoryId});
            _problemService.Create(new Problem {Name = "xyz", CategoryId = categoryId});

            Assert.Equal("abc", _context.Categories.First(c => c.Id == categoryId).Name);
            Assert.Equal(3, _categoryService.Get(categoryId).Problems.Count());

            _categoryService.Update(categoryId, new Category {Name = "xxx"});
            Assert.Equal("xxx", _context.Categories.First(c => c.Id == categoryId).Name);
            Assert.Equal(3, _categoryService.Get(categoryId).Problems.Count());
        }

        [Fact]
        public void GetCategoryTest()
        {
            var cid1 = _categoryService.Create(1, new Category {Name = "xxxx"});
            var cid2 = _categoryService.Create(cid1, new Category {Name = "yyyy"});

            var c1 = _categoryService.Get(cid1);
            Assert.Equal("xxxx", c1.Name);
            var cid3 = c1.Categories.First().Id;
            Assert.Equal(cid2, cid3);
            var c2 = _categoryService.Get(cid3);
            Assert.Equal("yyyy", c2.Name);
        }
    }
}