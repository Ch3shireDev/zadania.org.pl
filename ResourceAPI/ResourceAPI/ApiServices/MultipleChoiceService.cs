using System.Linq;
using ResourceAPI.Models.MultipleChoice;

namespace ResourceAPI.ApiServices
{
    public class MultipleChoiceService : IMultipleChoiceService
    {
        public MultipleChoiceService(SqlContext context)
        {
            Context = context;
        }

        private SqlContext Context { get; }

        public MultipleChoiceTest GetTestById(int testId, bool includeQuestions = false, bool includeAnswers = false)
        {
            var test = Context.MultipleChoiceTests.Select(t => new MultipleChoiceTest
            {
                Id = t.Id,
                AuthorId = t.AuthorId,
                Content = t.Content,
                Title = t.Title,
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
            var question = Context.MultipleChoiceQuestions
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
            var answer = Context.MultipleChoiceAnswers.Select(a => new MultipleChoiceAnswer
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
    }
}