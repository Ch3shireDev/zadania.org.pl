using System;
using System.Linq;
using CategoryLibrary;
using CommonLibrary;
using Microsoft.EntityFrameworkCore;
using QuizLibrary;
using ResourceAPI;
using ResourceAPI.ApiServices;
using Xunit;

namespace ResourceAPITests.QuizTests
{
    public class QuizServiceTests
    {
        public QuizServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString());
            _context = new SqlContext(optionsBuilder.Options);
            _categoryService = new CategoryService(_context);
            _QuizService = new QuizService(_context);
            _authorService = new AuthorService(_context);
        }

        private readonly SqlContext _context;
        private readonly ICategoryService _categoryService;
        private readonly IQuizService _QuizService;
        private IAuthorService _authorService;

        [Fact]
        public int CreateAnswer()
        {
            var questionId = CreateQuestion();
            var answerId = _QuizService.CreateAnswer(questionId, new QuizAnswer {Content = "xxx"});
            var answer = _QuizService.GetAnswer(answerId);
            Assert.Contains("xxx", answer.ContentHtml);
            return answerId;
        }

        [Fact]
        public int CreateQuestion()
        {
            var testId = CreateTest();

            // Tworzymy pytanie do testu.
            var questionId =
                _QuizService.CreateQuestion(testId, new QuizQuestion {Content = "xxx"});

            // Pobieramy test.
            var test = _QuizService.GetTest(testId);

            // Bierzemy pytanie z testu.
            var question = test.Questions.First();

            // Sprawdzamy, czy wartości są poprawne.
            Assert.Equal(questionId, question.Id);
            Assert.Contains("xxx", question.ContentHtml);

            // Pobieramy pytanie na drugi sposób.
            var question2 = _QuizService.GetQuestion(questionId);
            Assert.Equal(questionId, question2.Id);
            Assert.Contains("xxx", question2.ContentHtml);

            return questionId;
        }

        [Fact]
        public int CreateTest()
        {
            // Tworzymy test.
            var testId = _QuizService.CreateTest(1, new Quiz {Name = "abc", Content = "cde"});

            // Pobieramy test.
            var test = _QuizService.GetTest(testId);

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
            var answer1 = _QuizService.GetAnswer(answerId);
            Assert.NotNull(answer1);
            _QuizService.DeleteAnswer(answerId);
            var answer2 = _QuizService.GetAnswer(answerId);
            Assert.Null(answer2);
        }

        [Fact]
        public void DeleteQuestion()
        {
            var questionId = CreateQuestion();
            var question1 = _QuizService.GetQuestion(questionId);
            Assert.NotNull(question1);
            _QuizService.DeleteQuestion(questionId);
            var question2 = _QuizService.GetQuestion(questionId);
            Assert.Null(question2);
        }

        [Fact]
        public void DeleteTest()
        {
            var id = _categoryService.Create(1, new Category {Name = "xyz"});

            var test = _QuizService.CreateTest(id, new Quiz {Name = "abc"});

            var qid1 = _QuizService.CreateQuestion(test, new QuizQuestion {Content = "xyz"});
            var qid2 = _QuizService.CreateQuestion(test, new QuizQuestion {Content = "xyz"});
            var qid3 = _QuizService.CreateQuestion(test, new QuizQuestion {Content = "xyz"});

            var aid = _QuizService.CreateAnswer(qid1, new QuizAnswer {Content = "aaa"});

            var qnum = _QuizService.GetTest(test).Questions.Count;
            Assert.Equal(3, qnum);

            var testNum = _context.QuizTests.Count();
            var qNum = _context.QuizQuestions.Count();
            var aNum = _context.QuizAnswers.Count();

            _QuizService.DeleteTest(test);

            var testNum2 = _context.QuizTests.Count();
            var qNum2 = _context.QuizQuestions.Count();
            var aNum2 = _context.QuizAnswers.Count();

            Assert.Equal(testNum - 1, testNum2);
            Assert.Equal(qNum - 3, qNum2);
            Assert.Equal(aNum - 1, aNum2);
        }

        [Fact]
        public void EditAnswer()
        {
            var answerId = CreateAnswer();
            var answer = _QuizService.GetAnswer(answerId);
            answer.Content = "xxxxxxx";
            _QuizService.EditAnswer(answerId, answer);
            var answer2 = _QuizService.GetAnswer(answerId);
            Assert.Contains("xxxxxx", answer2.ContentHtml);
        }

        [Fact]
        public void EditQuestion()
        {
            var questionId = CreateQuestion();
            var question1 = _QuizService.GetQuestion(questionId);
            question1.Content = "xxxxxx";
            _QuizService.EditQuestion(questionId, question1);
        }

        [Fact]
        public void EditTest()
        {
            var testId = _QuizService.CreateTest(1, new Quiz {Name = "abc"});
            var question = _QuizService.CreateQuestion(testId, new QuizQuestion {Content = "xxx"});

            var test1 = _QuizService.GetTest(testId);
            Assert.Equal("abc", test1.Name);

            _QuizService.EditTest(testId, new Quiz {Name = "cde"});

            var test2 = _QuizService.GetTest(testId);
            Assert.Equal("cde", test2.Name);
        }
    }
}