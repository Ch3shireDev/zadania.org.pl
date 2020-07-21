using System.Net;
using System.Net.Http;
using Xunit;

namespace ResourceAPITests
{
    public class CategoryControllerTests
    {
        private HttpClient Client { get; } = new TestClientProvider().Client;

        [Fact]
        public async void BrowseTest()
        {
            var response = await Client.GetAsync("/api/v1/categories/");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}