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
            if (!response.IsSuccessStatusCode) return default;
            var task = Task.Run(async () => await response.Content.ReadAsStringAsync());
            var responseStr = task.Result;
            var element = JsonSerializer.Deserialize<T>(responseStr, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return element;
        }


        public static async Task<T> PostAsync<T>(this HttpClient client, string url, T element)
        {
            var res = await client.PostAsync(url, element.ToHttpContent());
            return res.ToElement<T>();
        }

        public static async Task<T> PutAsync<T>(this HttpClient client, string url, T element)
        {
            var res = await client.PutAsync(url, element.ToHttpContent());
            return res.ToElement<T>();
        }
    }
}