using CheckingPaymentService;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoolProject
{
    public class HttpRequestHandler
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> SendGetRequestAsync(string url)
        {
            try
            {
                LogHelper.WriteEvent($"Send GET request to the address: {url}", EventType.Info);
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                LogHelper.WriteEvent($"Send Get Request Error:{e.Message}", EventType.Error);
                return null;
            }
        }
        public async Task<string> SendPostRequestAsync(string url, HttpContent content)
        {
            try
            {
                LogHelper.WriteEvent($"Send POST request to the address: {url}", EventType.Info);
                HttpResponseMessage response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                LogHelper.WriteEvent($"Send Post Request Error:{e.Message}", EventType.Error);
                return null;
            }
        }
    }
}