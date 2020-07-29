using System;
using System.Linq;
using CategoryLibrary;
using CommonLibrary;
using Microsoft.EntityFrameworkCore;
using MultipleChoiceLibrary;
using ResourceAPI;
using ResourceAPI.ApiServices;
using Xunit;

namespace ResourceAPITests
{
    public class MultipleChoiceServiceTests
    {
        public MultipleChoiceServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("zadania");
            _context = new SqlContext(optionsBuilder.Options);
            _categoryService = new CategoryService(_context);
            _multipleChoiceService = new MultipleChoiceService(_context);
            _authorService = new AuthorService(_context);
        }

        private readonly SqlContext _context;
        private readonly ICategoryService _categoryService;
        private readonly IMultipleChoiceService _multipleChoiceService;
        private IAuthorService _authorService;

        [Fact]
        public void CreateAnswer()
        {
            var test = _multipleChoiceService.CreateTest(1, new MultipleChoiceTest {Name = "abc"});
            var question = _multipleChoiceService.CreateQuestion(test, new MultipleChoiceQuestion {Content = "xxx"});
            throw new NotImplementedException();
        }

        [Fact]
        public void CreateQuestion()
        {
            var test = _multipleChoiceService.CreateTest(1, new MultipleChoiceTest {Name = "abc"});
            var question = _multipleChoiceService.CreateQuestion(test, new MultipleChoiceQuestion {Content = "xxx"});
            throw new NotImplementedException();
        }

        [Fact]
        public void CreateTest()
        {
            var id = _categoryService.Create(1, new Category {Name = "xyz"});
            var initNum = _context.Categories.FirstOrDefault(c => c.Id == id)?.MultipleChoiceTests.ToList().Count;

            var test = _multipleChoiceService.CreateTest(id, new MultipleChoiceTest {Name = "abc"});
            var num = _context.Categories.FirstOrDefault(c => c.Id == id)?.MultipleChoiceTests.ToList().Count;
            Assert.Equal(initNum + 1, num);
        }

        [Fact]
        public void DeleteAnswer()
        {
            var test = _multipleChoiceService.CreateTest(1, new MultipleChoiceTest {Name = "abc"});
            var question = _multipleChoiceService.CreateQuestion(test, new MultipleChoiceQuestion {Content = "xxx"});
            throw new NotImplementedException();
        }

        [Fact]
        public void DeleteQuestion()
        {
            var test = _multipleChoiceService.CreateTest(1, new MultipleChoiceTest {Name = "abc"});
            var question = _multipleChoiceService.CreateQuestion(test, new MultipleChoiceQuestion {Content = "xxx"});
            throw new NotImplementedException();
        }

        [Fact]
        public void DeleteTest()
        {
            var id = _categoryService.Create(1, new Category {Name = "xyz"});

            var test = _multipleChoiceService.CreateTest(id, new MultipleChoiceTest {Name = "abc"});

            var qid1 = _multipleChoiceService.CreateQuestion(test, new MultipleChoiceQuestion {Content = "xyz"});
            var qid2 = _multipleChoiceService.CreateQuestion(test, new MultipleChoiceQuestion {Content = "xyz"});
            var qid3 = _multipleChoiceService.CreateQuestion(test, new MultipleChoiceQuestion {Content = "xyz"});

            var aid = _multipleChoiceService.CreateAnswer(qid1, new MultipleChoiceAnswer {Content = "aaa"});

            var qnum = _multipleChoiceService.GetTest(test).Questions.Count;
            Assert.Equal(3, qnum);

            var testNum = _context.MultipleChoiceTests.Count();
            var qNum = _context.MultipleChoiceQuestions.Count();
            var aNum = _context.MultipleChoiceAnswers.Count();

            _multipleChoiceService.DeleteTest(test);

            var testNum2 = _context.MultipleChoiceTests.Count();
            var qNum2 = _context.MultipleChoiceQuestions.Count();
            var aNum2 = _context.MultipleChoiceAnswers.Count();

            Assert.Equal(testNum - 1, testNum2);
            Assert.Equal(qNum - 3, qNum2);
            Assert.Equal(aNum - 1, aNum2);
        }

        [Fact]
        public void EditAnswer()
        {
            var test = _multipleChoiceService.CreateTest(1, new MultipleChoiceTest {Name = "abc"});
            var question = _multipleChoiceService.CreateQuestion(test, new MultipleChoiceQuestion {Content = "xxx"});
            throw new NotImplementedException();
        }

        [Fact]
        public void EditQuestion()
        {
            var test = _multipleChoiceService.CreateTest(1, new MultipleChoiceTest {Name = "abc"});
            var question = _multipleChoiceService.CreateQuestion(test, new MultipleChoiceQuestion {Content = "xxx"});
            throw new NotImplementedException();
        }

        [Fact]
        public void EditTest()
        {
            var testId = _multipleChoiceService.CreateTest(1, new MultipleChoiceTest {Name = "abc"});
            var question = _multipleChoiceService.CreateQuestion(testId, new MultipleChoiceQuestion {Content = "xxx"});

            var test1 = _multipleChoiceService.GetTest(testId);
            Assert.Equal("abc", test1.Name);

            _multipleChoiceService.EditTest(testId, new MultipleChoiceTest {Name = "cde"});

            var test2 = _multipleChoiceService.GetTest(testId);
            Assert.Equal("cde", test2.Name);
        }
    }
}