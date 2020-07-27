using System.Linq;
using ResourceAPI.ApiServices.Interfaces;
using ResourceAPI.Models.MultipleChoice;

namespace ResourceAPI.ApiServices
{
    public class MultipleChoiceService : IMultipleChoiceService
    {
        private readonly SqlContext _context;

        public MultipleChoiceService(SqlContext context)
        {
            _context = context;
        }

        public MultipleChoiceTest GetTestById(int testId, bool includeQuestions = false, bool includeAnswers = false)
        {
            var test = _context.MultipleChoiceTests.Select(t => new MultipleChoiceTest
            {
                Id = t.Id,
                AuthorId = t.AuthorId,
                Content = t.Content,
                Name = t.Name,
                Questions = t.Questions.Select(q => new MultipleChoiceQuestion {TestId = q.TestId, Id = q.Id}).ToList()
            }).FirstOrDefault(t => t.Id == testId);
            if (test == null) return null;
            test.Render();
            if (includeQuestions)
                test.Questions = test.Questions.Select(q => GetQuestionById(q.TestId, q.Id, includeAnswers)).ToList();
            return test;
        }

        public MultipleChoiceQuestion GetQuestionById(int testId, int questionId, bool includeAnswers = false)
        {
            var question = _context.MultipleChoiceQuestions
                .Select(q => new MultipleChoiceQuestion
                {
                    Id = q.Id,
                    AuthorId = q.AuthorId,
                    TestId = testId,
                    Content = q.Content,
                    Solution = q.Solution,
                    Answers = q.Answers.Select(a => new MultipleChoiceAnswer
                        {Id = a.Id, QuestionId = a.QuestionId, TestId = a.TestId}).ToList()
                })
                .FirstOrDefault(q => q.TestId == testId && q.Id == questionId);
            if (question == null) return null;
            question.Render();
            if (includeAnswers)
                question.Answers = question.Answers.Select(a => GetAnswerById(a.TestId, a.QuestionId, a.Id)).ToList();
            return question;
        }

        public MultipleChoiceAnswer GetAnswerById(int testId, int questionId, int answerId)
        {
            var answer = _context.MultipleChoiceAnswers.Select(a => new MultipleChoiceAnswer
            {
                Id = a.Id,
                QuestionId = a.QuestionId,
                TestId = a.Question.TestId,
                Content = a.Content,
                IsCorrect = a.IsCorrect
            }).FirstOrDefault(a => a.Id == answerId && a.QuestionId == questionId && a.TestId == testId);
            answer?.Render();
            return answer;
        }

        public int CreateTest(int categoryId, MultipleChoiceTest element, int authorId = 1)
        {
            if (!_context.Categories.Any(c => c.Id != categoryId)) return 0;
            if (!_context.Authors.Any(a => a.Id == authorId)) return 0;

            var test = new MultipleChoiceTest
            {
                Name = element.Name,
                CategoryId = categoryId,
                AuthorId = authorId
            };

            _context.MultipleChoiceTests.Add(test);
            _context.SaveChanges();
            return test.Id;
        }

        public int CreateQuestion(int testId, MultipleChoiceQuestion question, int authorId = 1)
        {
            var test = _context.MultipleChoiceTests.FirstOrDefault(t => t.Id == testId);
            if (test == null) return 0;
            if (!_context.Authors.Any(a => a.Id == authorId)) return 0;
            var newQuestion = new MultipleChoiceQuestion
            {
                Content = question.Content,
                TestId = testId,
                AuthorId = authorId
            };
            _context.MultipleChoiceQuestions.Add(newQuestion);
            _context.SaveChanges();
            return newQuestion.Id;
        }

        public int CreateAnswer(int questionId, MultipleChoiceAnswer answer, int authorId = 1)
        {
            var question = _context.MultipleChoiceQuestions.FirstOrDefault(q => q.Id == questionId);
            if (question == null) return 0;
            if (!_context.Authors.Any(a => a.Id == authorId)) return 0;
            var newAnswer = new MultipleChoiceAnswer
            {
                Content = answer.Content,
                QuestionId = questionId,
                AuthorId = authorId
            };
            _context.MultipleChoiceAnswers.Add(newAnswer);
            _context.SaveChanges();
            return newAnswer.Id;
        }

        public bool DeleteTest(int testId)
        {
            var test = _context.MultipleChoiceTests.FirstOrDefault(t => t.Id == testId);
            if (test == null) return false;
            _context.MultipleChoiceTests.Remove(test);
            _context.SaveChanges();
            return true;
        }

        public int Create(MultipleChoiceTest multipleChoiceTest, int authorId = 1)
        {
            var element = new MultipleChoiceTest
            {
                Name = multipleChoiceTest.Name,
                Content = multipleChoiceTest.Content,
                AuthorId = authorId,
                CategoryId = multipleChoiceTest.CategoryId
            };
            _context.MultipleChoiceTests.Add(element);
            _context.SaveChanges();
            return element.Id;
        }
    }
}