using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ResourceAPI.Models.Category;
using ResourceAPI.Models.Problem;
using Xunit;

namespace ResourceAPITests.ProblemTests
{
    public class ProblemsControllerTests
    {
        private readonly HttpClient _client = new TestClientProvider().Client;

        [Fact]
        public async Task BrowseTest()
        {
            var response = await _client.GetAsync("/api/v1/problems/");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProblemTest()
        {
            var problem = new Problem
            {
                Name = "abc",
                Content = "cde"
            };

            // Utworzenie problemu powinno zwrócić wartość id.
            var response = await _client.PostAsync("/api/v1/problems/", problem.ToHttpContent());
            response.EnsureSuccessStatusCode();
            var resProblem = response.ToElement<Problem>();

            // Problem powinien być pod wskazanym id.
            var response2 = await _client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            var probRes = response2.ToElement<Problem>();
            Assert.Equal("abc", probRes.Name);
            Assert.Contains("cde", probRes.ContentHtml);

            // Kasujemy zasób.
            var resDel = await _client.DeleteAsync($"/api/v1/problems/{resProblem.Id}");
            resDel.EnsureSuccessStatusCode();

            // Problemu powinno nie być pod wskazanym id.
            var response3 = await _client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            Assert.Equal(HttpStatusCode.NotFound, response3.StatusCode);
        }

        [Fact]
        public async Task EditProblemTest()
        {
            var problem = new Problem
            {
                Name = "abc",
                Content = "cde"
            };

            // Utworzenie problemu powinno zwrócić wartość id.
            var response = await _client.PostAsync("/api/v1/problems/", problem.ToHttpContent());
            response.EnsureSuccessStatusCode();
            var resProblem = response.ToElement<Problem>();

            // Bieżący problem powinien zawierać nowe wartości.
            var problem1Res = await _client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            var problem1 = problem1Res.ToElement<Problem>();
            Assert.Equal("abc", problem1.Name);
            Assert.Contains("cde", problem1.ContentHtml);

            // Zmieniamy parametry problemu.
            problem.Name = "xyz";
            problem.Content = "zzz";

            // Wysyłamy nową wersję.
            await _client.PutAsync($"/api/v1/problems/{resProblem.Id}", problem.ToHttpContent());

            // Bieżący problem powinien zawierać nowe wartości.
            var problem2Res = await _client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            var problem2 = problem2Res.ToElement<Problem>();
            Assert.Equal("xyz", problem2.Name);
            Assert.Contains("zzz", problem2.ContentHtml);
        }

        [Fact]
        public async Task PostProblemTest()
        {
            var problem = new Problem
            {
                Name = "abc",
                Content = "cde",
                ContentHtml = "cde"
            };

            // Utworzenie problemu powinno zwrócić wartość id.
            var response = await _client.PostAsync("/api/v1/problems/", problem.ToHttpContent());
            response.EnsureSuccessStatusCode();
            var resProblem = response.ToElement<Problem>();

            // Problem powinien być pod wskazanym id.
            var response2 = await _client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            var probRes = response2.ToElement<Problem>();
            Assert.Equal("abc", probRes.Name);
            Assert.Contains("cde", probRes.ContentHtml);

            // Problem powinien wylądować w kategorii 1, Root.
            var catRes = await _client.GetAsync("/api/v1/categories/1");
            var cat = catRes.ToElement<Category>();
            Assert.Equal("abc", cat.Problems.First().Name);
        }
    }
}