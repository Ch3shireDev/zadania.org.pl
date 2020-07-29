using System.Collections.Generic;
using System.Linq;

namespace MultipleChoiceLibrary
{
    public class MultipleChoiceService : IMultipleChoiceService
    {
        private readonly IMultipleChoiceDbContext _context;

        public MultipleChoiceService(IMultipleChoiceDbContext context)
        {
            _context = context;
        }

        public MultipleChoiceTest GetTest(int testId, bool includeQuestions = true, bool includeAnswers = true)
        {
            var test = _context.MultipleChoiceTests.Select(t => new MultipleChoiceTest
            {
                Id = t.Id,
                AuthorId = t.AuthorId,
                Content = t.Content,
                CategoryId = t.CategoryId,
                Name = t.Name,
                Questions = t.Questions.Select(q => new MultipleChoiceQuestion {Id = q.Id}).ToList()
            }).FirstOrDefault(t => t.Id == testId);
            if (test == null) return null;
            test.Render();
            if (includeQuestions)
                test.Questions = test.Questions.Select(q => GetQuestion(q.Id, includeAnswers)).ToList();
            return test;
        }

        public MultipleChoiceQuestion GetQuestion(int questionId, bool includeAnswers = false)
        {
            var question = _context.MultipleChoiceQuestions
                .Select(q => new MultipleChoiceQuestion
                {
                    Id = q.Id,
                    AuthorId = q.AuthorId,
                    TestId = q.TestId,
                    Content = q.Content,
                    Solution = q.Solution,
                    Answers = q.Answers.Select(a => new MultipleChoiceAnswer
                        {Id = a.Id, QuestionId = a.QuestionId, TestId = a.TestId}).ToList()
                })
                .FirstOrDefault(q => q.Id == questionId);
            if (question == null) return null;
            question.Render();
            if (includeAnswers)
                question.Answers = question.Answers.Select(a => GetAnswer(a.Id)).ToList();
            return question;
        }

        public MultipleChoiceAnswer GetAnswer(int answerId)
        {
            var answer = _context.MultipleChoiceAnswers.Select(a => new MultipleChoiceAnswer
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

        public int CreateTest(int categoryId, MultipleChoiceTest element, int authorId = 1)
        {
            //if (!_context.Categories.Any(c => c.Id != categoryId)) return 0;
            //if (!_context.Authors.Any(a => a.Id == authorId)) return 0;

            var test = new MultipleChoiceTest
            {
                Name = element.Name,
                Content = element.Content,
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
            //if (!_context.Authors.Any(a => a.Id == authorId)) return 0;
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
            //if (!_context.Authors.Any(a => a.Id == authorId)) return 0;
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

        public bool EditAnswer(int answerId, MultipleChoiceAnswer answer)
        {
            var element = _context.MultipleChoiceAnswers.FirstOrDefault(a => a.Id == answerId);
            if (element == null) return false;
            element.Content = answer.Content;
            _context.MultipleChoiceAnswers.Update(element);
            _context.SaveChanges();
            return true;
        }

        public bool EditQuestion(int questionId, MultipleChoiceQuestion question)
        {
            var element = _context.MultipleChoiceQuestions.FirstOrDefault(q => q.Id == questionId);
            if (element == null) return false;
            element.Content = question.Content;
            _context.MultipleChoiceQuestions.Update(element);
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<MultipleChoiceTest> Browse()
        {
            var multipleChoiceTests = _context.MultipleChoiceTests.ToList();
            return multipleChoiceTests;
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

        public bool EditTest(int testId, MultipleChoiceTest multipleChoiceTest)
        {
            var element = _context.MultipleChoiceTests.FirstOrDefault(m => m.Id == testId);
            if (element == null) return false;
            element.Name = multipleChoiceTest.Name;
            element.Content = multipleChoiceTest.Content;
            element.CategoryId = multipleChoiceTest.CategoryId;
            _context.MultipleChoiceTests.Update(element);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteAnswer(int answerId)
        {
            var answer = _context.MultipleChoiceAnswers.FirstOrDefault(a => a.Id == answerId);
            if (answer == null) return false;
            _context.MultipleChoiceAnswers.Remove(answer);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteQuestion(int questionId)
        {
            var question = _context.MultipleChoiceQuestions.FirstOrDefault(q => q.Id == questionId);
            if (question == null) return false;
            _context.MultipleChoiceQuestions.Remove(question);
            _context.SaveChanges();
            return true;
        }
    }
}