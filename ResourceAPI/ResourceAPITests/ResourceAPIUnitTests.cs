using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ResourceAPI.Models.Problem;
using Xunit;

namespace ResourceAPITests
{
    public class ResourceApiUnitTests
    {
        private HttpClient Client { get; } = new TestClientProvider().Client;

        [Fact]
        public async Task BrowseTest()
        {
            var response = await Client.GetAsync("/api/v1/problems/");
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
                ParentId = 1,
            };

            var jsonString = JsonSerializer.Serialize(problem);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            await Client.GetAsync("/api/v1/categories/1");
            var response = await Client.PostAsync("/api/v1/problems/", content);
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var response2 = await Client.GetAsync("/api/v1/problems/1");
            var responseStr = await response2.Content.ReadAsStringAsync();

            var problemResponse = JsonSerializer.Deserialize<Problem>(responseStr, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.Equal("abc", problemResponse.Title);
            Assert.Equal("cde", problemResponse.Content);
        }
    }
}