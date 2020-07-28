using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ResourceAPITests
{
    public static class TestTools
    {
        public static HttpContent ToHttpContent<T>(this T element)
        {
            var jsonString = JsonSerializer.Serialize(element);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return content;
        }

        public static T ToElement<T>(this HttpResponseMessage response)
        {
            var task = Task.Run(async () => await response.Content.ReadAsStringAsync());
            var responseStr = task.Result;
            var element = JsonSerializer.Deserialize<T>(responseStr, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return element;
        }
    }
}