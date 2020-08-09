using System;
using System.Linq;
using CategoryLibrary;
using CommonLibrary;
using Microsoft.EntityFrameworkCore;
using QuizLibrary;
using ResourceAPI;
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
            _quizService = new QuizService(_context);
            _authorService = new AuthorService(_context);
        }

        private readonly SqlContext _context;
        private readonly ICategoryService _categoryService;
        private readonly IQuizService _quizService;
        private IAuthorService _authorService;

        [Fact]
        public int CreateAnswer()
        {
            var questionId = CreateQuestion();
            var answerId = _quizService.CreateAnswer(questionId, new QuizAnswer {Content = "xxx"});
            var answer = _quizService.GetAnswer(answerId);
            Assert.Contains("xxx", answer.Content);
            return answerId;
        }

        [Fact]
        public int CreateQuestion()
        {
            var testId = CreateTest();

            // Tworzymy pytanie do testu.
            var questionId =
                _quizService.CreateQuestion(testId, new QuizQuestion {Content = "xxx"});

            // Pobieramy test.
            var test = _quizService.GetTest(testId);

            // Bierzemy pytanie z testu.
            var question = test.Questions.First();

            // Sprawdzamy, czy wartości są poprawne.
            Assert.Equal(questionId, question.Id);
            Assert.Contains("xxx", question.Content);

            // Pobieramy pytanie na drugi sposób.
            var question2 = _quizService.GetQuestion(questionId);
            Assert.Equal(questionId, question2.Id);
            Assert.Contains("xxx", question2.Content);

            return questionId;
        }

        [Fact]
        public int CreateTest()
        {
            // Tworzymy test.
            var testId = _quizService.CreateTest(1, new Quiz {Name = "abc", Content = "cde"});

            // Pobieramy test.
            var test = _quizService.GetTest(testId);

            // Sprawdzamy czy wartości Id i zawartość są poprawne.
            Assert.Equal(testId, test.Id);
            Assert.Equal("abc", test.Name);
            Assert.Contains("cde", test.Content);

            return testId;
        }

        [Fact]
        public void DeleteAnswer()
        {
            var answerId = CreateAnswer();
            var answer1 = _quizService.GetAnswer(answerId);
            Assert.NotNull(answer1);
            _quizService.DeleteAnswer(answerId);
            var answer2 = _quizService.GetAnswer(answerId);
            Assert.Null(answer2);
        }

        [Fact]
        public void DeleteQuestion()
        {
            var questionId = CreateQuestion();
            var question1 = _quizService.GetQuestion(questionId);
            Assert.NotNull(question1);
            _quizService.DeleteQuestion(questionId);
            var question2 = _quizService.GetQuestion(questionId);
            Assert.Null(question2);
        }

        [Fact]
        public void DeleteTest()
        {
            var id = _categoryService.Create(new Category {Name = "xyz"}).Id;

            var test = _quizService.CreateTest(id, new Quiz {Name = "abc"});

            var qid1 = _quizService.CreateQuestion(test, new QuizQuestion {Content = "xyz"});
            var qid2 = _quizService.CreateQuestion(test, new QuizQuestion {Content = "xyz"});
            var qid3 = _quizService.CreateQuestion(test, new QuizQuestion {Content = "xyz"});

            var aid = _quizService.CreateAnswer(qid1, new QuizAnswer {Content = "aaa"});

            var qnum = _quizService.GetTest(test).Questions.Count;
            Assert.Equal(3, qnum);

            var testNum = _context.QuizTests.Count();
            var qNum = _context.QuizQuestions.Count();
            var aNum = _context.QuizAnswers.Count();

            _quizService.DeleteTest(test);

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
            var answer = _quizService.GetAnswer(answerId);
            answer.Content = "xxxxxxx";
            _quizService.EditAnswer(answerId, answer);
            var answer2 = _quizService.GetAnswer(answerId);
            Assert.Contains("xxxxxx", answer2.Content);
        }

        [Fact]
        public void EditQuestion()
        {
            var questionId = CreateQuestion();
            var question1 = _quizService.GetQuestion(questionId);
            question1.Content = "xxxxxx";
            _quizService.EditQuestion(questionId, question1);
        }

        [Fact]
        public void EditTest()
        {
            var testId = _quizService.CreateTest(1, new Quiz {Name = "abc"});
            var question = _quizService.CreateQuestion(testId, new QuizQuestion {Content = "xxx"});

            var test1 = _quizService.GetTest(testId);
            Assert.Equal("abc", test1.Name);

            _quizService.EditTest(testId, new Quiz {Name = "cde"});

            var test2 = _quizService.GetTest(testId);
            Assert.Equal("cde", test2.Name);
        }

        [Fact]
        public void FileAnswerCreate()
        {
            throw new Exception();
        }

        [Fact]
        public void FileQuestionCreate()
        {
            throw new Exception();
        }

        [Fact]
        public void FileTestCreate()
        {
            throw new Exception();
        }
    }
}