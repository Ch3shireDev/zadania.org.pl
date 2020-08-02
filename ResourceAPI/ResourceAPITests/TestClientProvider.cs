using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using ResourceAPI;

namespace ResourceAPITests
{
    public class TestClientProvider
    {
        public TestClientProvider()
        {
            var builder = new WebHostBuilder().UseStartup<Startup>();
            builder.UseEnvironment("Development");
            var server = new TestServer(builder);
            Client = server.CreateClient();
        }

        public HttpClient Client { get; }
    }
}