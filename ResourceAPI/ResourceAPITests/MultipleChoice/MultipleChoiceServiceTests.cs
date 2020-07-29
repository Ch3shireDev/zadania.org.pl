using System;
using System.Linq;
using CategoryLibrary;
using CommonLibrary;
using Microsoft.EntityFrameworkCore;
using MultipleChoiceLibrary;
using ResourceAPI;
using ResourceAPI.ApiServices;
using Xunit;

namespace ResourceAPITests.MultipleChoice
{
    public class MultipleChoiceServiceTests
    {
        public MultipleChoiceServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString());
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
        public int CreateAnswer()
        {
            var questionId = CreateQuestion();
            var answerId = _multipleChoiceService.CreateAnswer(questionId, new MultipleChoiceAnswer {Content = "xxx"});
            var answer = _multipleChoiceService.GetAnswer(answerId);
            Assert.Contains("xxx", answer.ContentHtml);
            return answerId;
        }

        [Fact]
        public int CreateQuestion()
        {
            var testId = CreateTest();

            // Tworzymy pytanie do testu.
            var questionId =
                _multipleChoiceService.CreateQuestion(testId, new MultipleChoiceQuestion {Content = "xxx"});

            // Pobieramy test.
            var test = _multipleChoiceService.GetTest(testId);

            // Bierzemy pytanie z testu.
            var question = test.Questions.First();

            // Sprawdzamy, czy wartości są poprawne.
            Assert.Equal(questionId, question.Id);
            Assert.Contains("xxx", question.ContentHtml);

            // Pobieramy pytanie na drugi sposób.
            var question2 = _multipleChoiceService.GetQuestion(questionId);
            Assert.Equal(questionId, question2.Id);
            Assert.Contains("xxx", question2.ContentHtml);

            return questionId;
        }

        [Fact]
        public int CreateTest()
        {
            // Tworzymy test.
            var testId = _multipleChoiceService.CreateTest(1, new MultipleChoiceTest {Name = "abc", Content = "cde"});

            // Pobieramy test.
            var test = _multipleChoiceService.GetTest(testId);

            // Sprawdzamy czy wartości Id i zawartość są poprawne.
            Assert.Equal(testId, test.Id);
            Assert.Equal("abc", test.Name);
            Assert.Contains("cde", test.ContentHtml);

            return testId;
        }

        [Fact]
        public void DeleteAnswer()
        {
            var answerId = CreateAnswer();
            var answer1 = _multipleChoiceService.GetAnswer(answerId);
            Assert.NotNull(answer1);
            _multipleChoiceService.DeleteAnswer(answerId);
            var answer2 = _multipleChoiceService.GetAnswer(answerId);
            Assert.Null(answer2);
        }

        [Fact]
        public void DeleteQuestion()
        {
            var questionId = CreateQuestion();
            var question1 = _multipleChoiceService.GetQuestion(questionId);
            Assert.NotNull(question1);
            _multipleChoiceService.DeleteQuestion(questionId);
            var question2 = _multipleChoiceService.GetQuestion(questionId);
            Assert.Null(question2);
        }

        [Fact]
        public void DeleteTest()
        {
            var id = _categoryService.Create(1, new CategoryLibrary.Category {Name = "xyz"});

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
            var answerId = CreateAnswer();
            var answer = _multipleChoiceService.GetAnswer(answerId);
            answer.Content = "xxxxxxx";
            _multipleChoiceService.EditAnswer(answerId, answer);
            var answer2 = _multipleChoiceService.GetAnswer(answerId);
            Assert.Contains("xxxxxx", answer2.ContentHtml);
        }

        [Fact]
        public void EditQuestion()
        {
            var questionId = CreateQuestion();
            var question1 = _multipleChoiceService.GetQuestion(questionId);
            question1.Content = "xxxxxx";
            _multipleChoiceService.EditQuestion(questionId, question1);
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