using System.Collections.Generic;
using System.Linq;
using FileDataLibrary;

namespace QuizLibrary
{
    public class QuizService : IQuizService
    {
        private readonly IQuizDbContext _context;
        private readonly IFileDataService _fileDataService;

        public QuizService(IQuizDbContext context, IFileDataService fileDataService = null)
        {
            _context = context;
            _fileDataService = fileDataService;
        }

        public bool DeleteTest(int testId)
        {
            var test = _context.QuizTests.FirstOrDefault(t => t.Id == testId);
            if (test == null) return false;
            _context.QuizTests.Remove(test);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteAnswer(int answerId)
        {
            var answer = _context.QuizAnswers.FirstOrDefault(a => a.Id == answerId);
            if (answer == null) return false;
            _context.QuizAnswers.Remove(answer);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteQuestion(int questionId)
        {
            var question = _context.QuizQuestions.FirstOrDefault(q => q.Id == questionId);
            if (question == null) return false;
            _context.QuizQuestions.Remove(question);
            _context.SaveChanges();
            return true;
        }

        public Quiz GetTest(int testId, bool includeQuestions = true, bool includeAnswers = true)
        {
            var test = _context.QuizTests.Select(t => new Quiz
            {
                Id = t.Id,
                AuthorId = t.AuthorId,
                Content = t.Content,
                CategoryId = t.CategoryId,
                Name = t.Name,
                Questions = t.Questions.Select(q => new QuizQuestion {Id = q.Id}).ToList()
            }).FirstOrDefault(t => t.Id == testId);
            if (test == null) return null;
            //test.Render();
            if (includeQuestions)
                test.Questions = test.Questions.Select(q => GetQuestion(q.Id, includeAnswers)).ToList();
            return test;
        }

        public QuizQuestion GetQuestion(int questionId, bool includeAnswers = false)
        {
            var question = _context.QuizQuestions
                .Select(q => new QuizQuestion
                {
                    Id = q.Id,
                    AuthorId = q.AuthorId,
                    TestId = q.TestId,
                    Content = q.Content,
                    Solution = q.Solution,
                    Answers = q.Answers.Select(a => new QuizAnswer
                        {Id = a.Id, QuestionId = a.QuestionId, TestId = a.TestId}).ToList()
                })
                .FirstOrDefault(q => q.Id == questionId);
            if (question == null) return null;
            question.Render();
            if (includeAnswers)
                question.Answers = question.Answers.Select(a => GetAnswer(a.Id)).ToList();
            return question;
        }

        public QuizAnswer GetAnswer(int answerId)
        {
            var answer = _context.QuizAnswers.Select(a => new QuizAnswer
            {
                Id = a.Id,
                QuestionId = a.QuestionId,
                TestId = a.Question.TestId,
                Content = a.Content,
                IsCorrect = a.IsCorrect
            }).FirstOrDefault(a => a.Id == answerId);
            answer?.Render();
            return answer;
        }

        public int CreateTest(int categoryId, Quiz element, int authorId = 1)
        {
            var test = new Quiz
            {
                Name = element.Name,
                Content = element.Content,
                CategoryId = categoryId,
                AuthorId = authorId
            };

            _context.QuizTests.Add(test);
            _context.SaveChanges();
            return test.Id;
        }

        public int CreateQuestion(int testId, QuizQuestion question, int authorId = 1)
        {
            var test = _context.QuizTests.FirstOrDefault(t => t.Id == testId);
            if (test == null) return 0;
            var newQuestion = new QuizQuestion
            {
                Content = question.Content,
                TestId = testId,
                AuthorId = authorId
            };
            _context.QuizQuestions.Add(newQuestion);
            _context.SaveChanges();
            return newQuestion.Id;
        }

        public int CreateAnswer(int questionId, QuizAnswer answer, int authorId = 1)
        {
            var question = _context.QuizQuestions.FirstOrDefault(q => q.Id == questionId);
            if (question == null) return 0;
            var newAnswer = new QuizAnswer
            {
                Content = answer.Content,
                QuestionId = questionId,
                AuthorId = authorId
            };
            _context.QuizAnswers.Add(newAnswer);
            _context.SaveChanges();
            return newAnswer.Id;
        }

        public bool EditAnswer(int answerId, QuizAnswer answer)
        {
            var element = _context.QuizAnswers.FirstOrDefault(a => a.Id == answerId);
            if (element == null) return false;
            element.Content = answer.Content;
            _context.QuizAnswers.Update(element);
            _context.SaveChanges();
            return true;
        }

        public bool EditQuestion(int questionId, QuizQuestion question)
        {
            var element = _context.QuizQuestions.FirstOrDefault(q => q.Id == questionId);
            if (element == null) return false;
            element.Content = question.Content;
            _context.QuizQuestions.Update(element);
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<Quiz> Browse()
        {
            var QuizTests = _context.QuizTests.ToList();
            return QuizTests;
        }

        public int Create(Quiz quiz, int authorId = 1)
        {
            if (quiz.CategoryId == 0) quiz.CategoryId = 1;

            var element = new Quiz
            {
                Name = quiz.Name,
                Content = quiz.Content,
                AuthorId = authorId,
                CategoryId = quiz.CategoryId,
                CanBeRandomized = quiz.CanBeRandomized
            };


            _context.QuizTests.Add(element);
            _context.SaveChanges();

            foreach (var file in quiz.Files) _fileDataService.CreateForQuizTest(file, element.Id);

            return element.Id;
        }

        public bool EditTest(int testId, Quiz quiz)
        {
            var element = _context.QuizTests.FirstOrDefault(m => m.Id == testId);
            if (element == null) return false;
            element.Name = quiz.Name;
            element.Content = quiz.Content;
            element.CategoryId = quiz.CategoryId;
            _context.QuizTests.Update(element);
            _context.SaveChanges();
            return true;
        }
    }
}