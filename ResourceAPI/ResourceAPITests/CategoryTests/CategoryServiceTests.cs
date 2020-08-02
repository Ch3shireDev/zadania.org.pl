using System;
using System.Linq;
using AutoMapper;
using CategoryLibrary;
using CommonLibrary;
using ExerciseLibrary;
using Microsoft.EntityFrameworkCore;
using ProblemLibrary;
using QuizLibrary;
using ResourceAPI;
using Xunit;

namespace ResourceAPITests.CategoryTests
{
    public class CategoryServiceTests
    {
        public CategoryServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString());

            var conf = new MapperConfiguration(c => { });
            var mapper = new Mapper(conf);

            _context = new SqlContext(optionsBuilder.Options);
            _categoryService = new CategoryService(_context);
            _problemService = new ProblemService(_context);
            _QuizService = new QuizService(_context);
            _exerciseService = new ExerciseService(_context);
            _authorService = new AuthorService(_context);
        }

        private readonly SqlContext _context;
        private readonly IExerciseService _exerciseService;
        private readonly ICategoryService _categoryService;
        private readonly IProblemService _problemService;
        private readonly IAuthorService _authorService;
        private readonly IQuizService _QuizService;

        [Fact]
        public void AddProblemsTest()
        {
            var id = _categoryService.Create(new Category {Name = "abc"}).Id;
            _categoryService.Create(new Category {Name = "cde"}, id);
            _categoryService.Create(new Category {Name = "xyz"}, id);
            var id2 = _categoryService.Create(new Category {Name = "xyz"}, id).Id;

            var pid0 = _problemService.Create(new Problem {Name = "xxx1", CategoryId = id2});
            var pid1 = _problemService.Create(new Problem {Name = "xxx2", CategoryId = id2});
            var pid2 = _problemService.Create(new Problem {Name = "xxx3", CategoryId = id2});

            var problems = _categoryService.GetProblems(id2);

            //Assert.Equal("xyz", category.Name);
            Assert.Equal(3, problems.Count());
            //var problems = category.Problems.ToList();
            Assert.Contains(problems, p => p.Id == pid0);
            Assert.Contains(problems, p => p.Id == pid1);
            Assert.Contains(problems, p => p.Id == pid2);
        }

        [Fact]
        public void AddQuizTest()
        {
            //Thread.Sleep(500);
            var id = _categoryService.Create(new Category {Name = "abc"}).Id;
            _categoryService.Create(new Category {Name = "cde"}, id);
            _categoryService.Create(new Category {Name = "xyz"}, id);
            var id2 = _categoryService.Create(new Category {Name = "xyz"}, id).Id;

            var pid0 = _QuizService.CreateTest(id2, new Quiz {Name = "xxx1"});
            var pid1 = _QuizService.CreateTest(id2, new Quiz {Name = "xxx2"});
            var pid2 = _QuizService.CreateTest(id2, new Quiz {Name = "xxx3"});

            var quizzes = _categoryService.GetQuizzes(id2);

            //Assert.Equal("xyz", category.Name);
            Assert.Equal(3, quizzes.Count());
            var tests = quizzes.ToList();
            Assert.Contains(tests, p => p.Id == pid0);
            Assert.Contains(tests, p => p.Id == pid1);
            Assert.Contains(tests, p => p.Id == pid2);
        }

        [Fact]
        public void BrowseCategoryTest()
        {
            var id = _categoryService.Create(new Category {Name = "abc"}).Id;
            _categoryService.Create(new Category {Name = "cde"}, id);
            _categoryService.Create(new Category {Name = "xyz"}, id);
            _categoryService.Create(new Category {Name = "xyz"}, id);

            var category = _categoryService.GetCategory(id);
            Assert.Equal(3, category.Categories.Count());
        }

        [Fact]
        public void CascadeDeleteTest()
        {
            //Thread.Sleep(1500);

            var first = _categoryService.Create(new Category()).Id;
            var second = _categoryService.Create(new Category(), first).Id;
            var third = _categoryService.Create(new Category(), second).Id;
            var fourth = _categoryService.Create(new Category(), third);
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

            var id = _categoryService.Create(category).Id;
            var root = _categoryService.GetProblems(1);
            var child = _categoryService.GetProblems(id);

            Assert.NotNull(root);
            Assert.NotNull(child);
            //Assert.Equal(child.Name, name);
        }


        [Fact]
        public void CreateTest()
        {
            var id = _categoryService.Create(new Category {Name = "xyz"}).Id;
            var initNum = _context.Categories.FirstOrDefault(c => c.Id == id)?.Quizzes.ToList().Count;

            var test = _QuizService.CreateTest(id, new Quiz {Name = "abc"});
            var num = _context.Categories.FirstOrDefault(c => c.Id == id)?.Quizzes.ToList().Count;
            Assert.Equal(initNum + 1, num);
        }

        [Fact]
        public void DeleteCategoryTest()
        {
            var categoryId = _categoryService.Create(new Category {Name = "abc"}).Id;

            var initial = _categoryService.GetProblems(categoryId).Count();

            _problemService.Create(new Problem {Name = "xyz", CategoryId = categoryId});
            _problemService.Create(new Problem {Name = "xyz", CategoryId = categoryId});
            _problemService.Create(new Problem {Name = "xyz", CategoryId = categoryId});

            Assert.Equal("abc", _context.Categories.First(c => c.Id == categoryId).Name);
            Assert.Equal(initial + 3, _categoryService.GetProblems(categoryId).Count());

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
            var categoryId = _categoryService.Create(new Category {Name = "abc"}).Id;

            _problemService.Create(new Problem {Name = "xyz", CategoryId = categoryId});
            _problemService.Create(new Problem {Name = "xyz", CategoryId = categoryId});
            _problemService.Create(new Problem {Name = "xyz", CategoryId = categoryId});

            Assert.Equal("abc", _context.Categories.First(c => c.Id == categoryId).Name);
            Assert.Equal(3, _categoryService.GetProblems(categoryId).Count());

            _categoryService.Update(new Category {Name = "xxx"}, categoryId);
            Assert.Equal("xxx", _context.Categories.First(c => c.Id == categoryId).Name);
            Assert.Equal(3, _categoryService.GetProblems(categoryId).Count());
        }

        [Fact]
        public void GetCategoryExercises()
        {
            var cid1 = _categoryService.Create(new Category {Name = "xxxx"}).Id;

            var pid1 = _exerciseService.Create(new Exercise {Name = "aaa", CategoryId = cid1});
            var pid2 = _exerciseService.Create(new Exercise {Name = "bbb", CategoryId = cid1});
            var pid3 = _exerciseService.Create(new Exercise {Name = "ccc", CategoryId = cid1});

            var exercises = _categoryService.GetExercises(cid1);

            var names = exercises.Select(p => p.Name).ToList();

            Assert.Equal(3, exercises.Count());

            Assert.Contains("aaa", names);
            Assert.Contains("bbb", names);
            Assert.Contains("ccc", names);
        }

        [Fact]
        public void GetCategoryProblems()
        {
            var cid1 = _categoryService.Create(new Category {Name = "xxxx"}).Id;

            var pid1 = _problemService.Create(new Problem {Name = "aaa", CategoryId = cid1});
            var pid2 = _problemService.Create(new Problem {Name = "bbb", CategoryId = cid1});
            var pid3 = _problemService.Create(new Problem {Name = "ccc", CategoryId = cid1});

            var category = _categoryService.GetProblems(cid1);

            var names = category.Select(p => p.Name).ToList();

            Assert.Equal(3, category.Count());

            Assert.Contains("aaa", names);
            Assert.Contains("bbb", names);
            Assert.Contains("ccc", names);
        }

        [Fact]
        public void GetCategoryQuizTests()
        {
            var cid1 = _categoryService.Create(new Category {Name = "xxxx"}).Id;

            var pid1 = _QuizService.Create(new Quiz {Name = "aaa", CategoryId = cid1});
            var pid2 = _QuizService.Create(new Quiz {Name = "bbb", CategoryId = cid1});
            var pid3 = _QuizService.Create(new Quiz {Name = "ccc", CategoryId = cid1});

            var quizzes = _categoryService.GetQuizzes(cid1);

            var names = quizzes.Select(p => p.Name).ToList();

            Assert.Equal(3, quizzes.Count());

            Assert.Contains("aaa", names);
            Assert.Contains("bbb", names);
            Assert.Contains("ccc", names);
        }

        [Fact]
        public void GetCategoryTest()
        {
            var cid1 = _categoryService.Create(new Category {Name = "xxxx"}).Id;
            var cid2 = _categoryService.Create(new Category {Name = "yyyy"}, cid1).Id;

            var c1 = _categoryService.GetCategory(cid1);
            //Assert.Equal("xxxx", c1.Name);
            var cid3 = c1.Categories.First().Id;
            Assert.Equal(cid2, cid3);
            var c2 = _categoryService.GetProblems(cid3);
            //Assert.Equal("yyyy", c2.Name);
        }
    }
}