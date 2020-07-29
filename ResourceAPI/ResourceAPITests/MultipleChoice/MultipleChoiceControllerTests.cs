using System.Net.Http;
using System.Threading.Tasks;
using MultipleChoiceLibrary;
using Xunit;

namespace ResourceAPITests.MultipleChoice
{
    public class MultipleChoiceControllerTests
    {
        private HttpClient Client { get; } = new TestClientProvider().Client;

        public async Task<MultipleChoiceAnswer> GetAnswer(int testId, int questionId, int answerId)
        {
            var res = await Client.GetAsync(
                $"/api/v1/multiple-choice-tests/{testId}/questions/{questionId}/answers/{answerId}");
            var answer = res.ToElement<MultipleChoiceAnswer>();
            return answer;
        }

        public async Task<MultipleChoiceQuestion> GetQuestion(int testId, int questionId)
        {
            var res = await Client.GetAsync($"/api/v1/multiple-choice-tests/{testId}/questions/{questionId}");
            var question = res.ToElement<MultipleChoiceQuestion>();
            return question;
        }

        public async Task<MultipleChoiceTest> GetTest(int testId)
        {
            var res = await Client.GetAsync($"/api/v1/multiple-choice-tests/{testId}");
            var test = res.ToElement<MultipleChoiceTest>();
            return test;
        }

        [Fact]
        public async Task<MultipleChoiceAnswer> CreateAnswer()
        {
            var test = await Client.PostAsync("/api/v1/multiple-choice-tests",
                new MultipleChoiceTest {Content = "abc", Name = "xyz"});
            var questionRes = await Client.PostAsync($"/api/v1/multiple-choice-tests/{test.Id}/questions",
                new MultipleChoiceQuestion {Content = "aaa"}.ToHttpContent());
            var question = questionRes.ToElement<MultipleChoiceQuestion>();
            var answer = await Client.PostAsync(
                $"/api/v1/multiple-choice-tests/{test.Id}/questions/{question.Id}/answers",
                new MultipleChoiceAnswer {Content = "bbb"});
            var answer1 = await GetAnswer(test.Id, question.Id, answer.Id);
            Assert.Contains("bbb", answer1.ContentHtml);
            return answer1;
        }

        [Fact]
        public async Task<MultipleChoiceQuestion> CreateQuestion()
        {
            var test = await Client.PostAsync("/api/v1/multiple-choice-tests",
                new MultipleChoiceTest {Content = "abc", Name = "xyz"});
            var question = await Client.PostAsync($"/api/v1/multiple-choice-tests/{test.Id}/questions",
                new MultipleChoiceQuestion {Content = "aaa"});
            var question1 = await GetQuestion(test.Id, question.Id);
            Assert.Contains("aaa", question1.ContentHtml);
            return question1;
        }

        [Fact]
        public async Task<MultipleChoiceTest> CreateTest()
        {
            var test = await Client.PostAsync("/api/v1/multiple-choice-tests",
                new MultipleChoiceTest {Content = "abc", Name = "xyz"});
            var test1 = await GetTest(test.Id);
            Assert.Equal("xyz", test1.Name);
            Assert.Contains("abc", test1.ContentHtml);
            return test1;
        }

        [Fact]
        public async void DeleteAnswer()
        {
            var answer = await CreateAnswer();
            Assert.NotNull(answer);
            await Client.DeleteAsync(
                $"/api/v1/multiple-choice-tests/{answer.TestId}/questions/{answer.QuestionId}/answers/{answer.Id}");
            var answer1 = await GetAnswer(answer.TestId, answer.QuestionId, answer.Id);
            Assert.Null(answer1);
        }

        [Fact]
        public async void DeleteQuestion()
        {
            var question = await CreateQuestion();
            Assert.NotNull(question);
            await Client.DeleteAsync($"/api/v1/multiple-choice-tests/{question.TestId}/questions/{question.Id}");
            var question1 = await GetQuestion(question.TestId, question.Id);
            Assert.Null(question1);
        }

        [Fact]
        public async void DeleteTest()
        {
            var test = await CreateTest();
            Assert.NotNull(test);
            await Client.DeleteAsync($"/api/v1/multiple-choice-tests/{test.Id}");
            var test1 = await GetTest(test.Id);
            Assert.Null(test1);
        }

        [Fact]
        public async void EditAnswer()
        {
            var answer = await CreateAnswer();
            Assert.NotNull(answer);
            await Client.PutAsync(
                $"/api/v1/multiple-choice-tests/{answer.TestId}/questions/{answer.QuestionId}/answers/{answer.Id}",
                new MultipleChoiceAnswer
                {
                    Content = "xxxaaa"
                }.ToHttpContent());
            var answer1 = await GetAnswer(answer.TestId, answer.QuestionId, answer.Id);
            Assert.Contains("xxxaaa", answer1.ContentHtml);
        }

        [Fact]
        public async void EditQuestion()
        {
            var question = await CreateQuestion();
            Assert.NotNull(question);
            await Client.PutAsync($"/api/v1/multiple-choice-tests/{question.TestId}/questions/{question.Id}",
                new MultipleChoiceQuestion
                {
                    Content = "aaabbb"
                }.ToHttpContent());
            var question1 = await GetQuestion(question.TestId, question.Id);
            Assert.Contains("aaabbb", question1.ContentHtml);
        }

        [Fact]
        public async void EditTest()
        {
            var test = await CreateTest();
            Assert.NotNull(test);
            await Client.PutAsync($"/api/v1/multiple-choice-tests/{test.Id}",
                new MultipleChoiceTest {Name = "aaabbb", Content = "cccddd"}.ToHttpContent());
            var test1 = await GetTest(test.Id);
            Assert.Equal("aaabbb", test1.Name);
            Assert.Contains("cccddd", test1.ContentHtml);
        }
    }
}