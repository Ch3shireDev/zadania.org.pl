using System.Net.Http;
using System.Threading.Tasks;
using QuizLibrary;
using Xunit;

namespace ResourceAPITests.QuizTests
{
    public class QuizControllerTests
    {
        private HttpClient Client { get; } = new TestClientProvider().Client;

        public async Task<QuizAnswer> GetAnswer(int testId, int questionId, int answerId)
        {
            var res = await Client.GetAsync(
                $"/api/v1/quiz/{testId}/questions/{questionId}/answers/{answerId}");
            var answer = res.ToElement<QuizAnswer>();
            return answer;
        }

        public async Task<QuizQuestion> GetQuestion(int testId, int questionId)
        {
            var res = await Client.GetAsync($"/api/v1/quiz/{testId}/questions/{questionId}");
            var question = res.ToElement<QuizQuestion>();
            return question;
        }

        public async Task<Quiz> GetTest(int testId)
        {
            var res = await Client.GetAsync($"/api/v1/quiz/{testId}");
            var test = res.ToElement<Quiz>();
            return test;
        }

        [Fact]
        public async Task<QuizAnswer> CreateAnswer()
        {
            var test = await Client.PostAsync("/api/v1/quiz",
                new Quiz {Content = "abc", Name = "xyz"});
            var questionRes = await Client.PostAsync($"/api/v1/quiz/{test.Id}/questions",
                new QuizQuestion {Content = "aaa"}.ToHttpContent());
            var question = questionRes.ToElement<QuizQuestion>();
            var answer = await Client.PostAsync(
                $"/api/v1/quiz/{test.Id}/questions/{question.Id}/answers",
                new QuizAnswer {Content = "bbb"});
            var answer1 = await GetAnswer(test.Id, question.Id, answer.Id);
            Assert.Contains("bbb", answer1.Content);
            return answer1;
        }

        [Fact]
        public async Task<QuizQuestion> CreateQuestion()
        {
            var test = await Client.PostAsync("/api/v1/quiz",
                new Quiz {Content = "abc", Name = "xyz"});
            var question = await Client.PostAsync($"/api/v1/quiz/{test.Id}/questions",
                new QuizQuestion {Content = "aaa"});
            var question1 = await GetQuestion(test.Id, question.Id);
            Assert.Contains("aaa", question1.Content);
            return question1;
        }

        [Fact]
        public async Task<Quiz> CreateTest()
        {
            var test = await Client.PostAsync("/api/v1/quiz",
                new Quiz {Content = "abc", Name = "xyz"});
            var test1 = await GetTest(test.Id);
            Assert.Equal("xyz", test1.Name);
            Assert.Contains("abc", test1.Content);
            return test1;
        }

        [Fact]
        public async void DeleteAnswer()
        {
            var answer = await CreateAnswer();
            Assert.NotNull(answer);
            await Client.DeleteAsync(
                $"/api/v1/quiz/{answer.TestId}/questions/{answer.QuestionId}/answers/{answer.Id}");
            var answer1 = await GetAnswer(answer.TestId, answer.QuestionId, answer.Id);
            Assert.Null(answer1);
        }

        [Fact]
        public async void DeleteQuestion()
        {
            var question = await CreateQuestion();
            Assert.NotNull(question);
            await Client.DeleteAsync($"/api/v1/quiz/{question.TestId}/questions/{question.Id}");
            var question1 = await GetQuestion(question.TestId, question.Id);
            Assert.Null(question1);
        }

        [Fact]
        public async void DeleteTest()
        {
            var test = await CreateTest();
            Assert.NotNull(test);
            await Client.DeleteAsync($"/api/v1/quiz/{test.Id}");
            var test1 = await GetTest(test.Id);
            Assert.Null(test1);
        }

        [Fact]
        public async void EditAnswer()
        {
            var answer = await CreateAnswer();
            Assert.NotNull(answer);
            await Client.PutAsync(
                $"/api/v1/quiz/{answer.TestId}/questions/{answer.QuestionId}/answers/{answer.Id}",
                new QuizAnswer
                {
                    Content = "xxxaaa"
                }.ToHttpContent());
            var answer1 = await GetAnswer(answer.TestId, answer.QuestionId, answer.Id);
            Assert.Contains("xxxaaa", answer1.Content);
        }

        [Fact]
        public async void EditQuestion()
        {
            var question = await CreateQuestion();
            Assert.NotNull(question);
            await Client.PutAsync($"/api/v1/quiz/{question.TestId}/questions/{question.Id}",
                new QuizQuestion
                {
                    Content = "aaabbb"
                }.ToHttpContent());
            var question1 = await GetQuestion(question.TestId, question.Id);
            Assert.Contains("aaabbb", question1.Content);
        }

        [Fact]
        public async void EditTest()
        {
            var test = await CreateTest();
            Assert.NotNull(test);
            await Client.PutAsync($"/api/v1/quiz/{test.Id}",
                new Quiz {Name = "aaabbb", Content = "cccddd"}.ToHttpContent());
            var test1 = await GetTest(test.Id);
            Assert.Equal("aaabbb", test1.Name);
            Assert.Contains("cccddd", test1.Content);
        }
    }
}