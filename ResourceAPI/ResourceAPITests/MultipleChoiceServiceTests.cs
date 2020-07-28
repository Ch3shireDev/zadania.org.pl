using System.Linq;
using CategoryLibrary;
using CommonLibrary;
using Microsoft.EntityFrameworkCore;
using MultipleChoiceLibrary;
using ResourceAPI;
using ResourceAPI.ApiServices;
using ResourceAPI.ApiServices.Interfaces;
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
        public void CreateTest()
        {
            var id = _categoryService.Create(1, new Category {Name = "xyz"});
            var initNum = _context.Categories.FirstOrDefault(c => c.Id == id)?.MultipleChoiceTests.ToList().Count;

            var test = _multipleChoiceService.CreateTest(id, new MultipleChoiceTest {Name = "abc"});
            var num = _context.Categories.FirstOrDefault(c => c.Id == id)?.MultipleChoiceTests.ToList().Count;
            Assert.Equal(initNum + 1, num);
        }

        [Fact]
        public void DeleteTest()
        {
            //Thread.Sleep(1000);

            var id = _categoryService.Create(1, new Category {Name = "xyz"});

            var test = _multipleChoiceService.CreateTest(id, new MultipleChoiceTest {Name = "abc"});

            var qid1 = _multipleChoiceService.CreateQuestion(test, new MultipleChoiceQuestion {Content = "xyz"});
            var qid2 = _multipleChoiceService.CreateQuestion(test, new MultipleChoiceQuestion {Content = "xyz"});
            var qid3 = _multipleChoiceService.CreateQuestion(test, new MultipleChoiceQuestion {Content = "xyz"});

            var aid = _multipleChoiceService.CreateAnswer(qid1, new MultipleChoiceAnswer {Content = "aaa"});

            var qnum = _multipleChoiceService.GetTestById(test).Questions.Count;
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
    }
}