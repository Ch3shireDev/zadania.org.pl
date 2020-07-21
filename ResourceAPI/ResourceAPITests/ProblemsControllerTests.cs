using System.Net;
using System.Threading.Tasks;
using ResourceAPI.Models.Problem;
using Xunit;

namespace ResourceAPITests
{
    public class ProblemsControllerTests
    {
        [Fact]
        public async Task BrowseTest()
        {
            using var client = new TestClientProvider().Client;
            var response = await client.GetAsync("/api/v1/problems/");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostProblemTest()
        {
            var problem = new Problem
            {
                Title = "abc",
                Content = "cde",
                ContentHtml = "cde",
                CategoryId = 1
            };

            using var client = new TestClientProvider().Client;
            await client.GetAsync("/api/v1/categories/1");
            var response = await client.PostAsync("/api/v1/problems/", problem.ToHttpContent());
            response.EnsureSuccessStatusCode();
            var resProblem = response.ToElement<Problem>();
            var response2 = await client.GetAsync($"/api/v1/problems/{resProblem.Id}");
            Assert.Equal("abc", response2.ToElement<Problem>().Title);
        }
    }
}