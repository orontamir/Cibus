using Newtonsoft.Json;
using Serilog;
using System.Net.Http.Headers;
using System.Text;

namespace CibusServer.Services
{
    public class HttpService
    {
        private readonly HttpClient _Client;
        private string _bearer = null;
        public HttpService(HttpClient client)
        {
            _Client = client;
            _Client.Timeout = TimeSpan.FromMilliseconds(30000);

        }

        public string BaseAddress
        {
            set
            {
                Log.Information($"Set base url for http client: {value}");
                _Client.BaseAddress = new Uri(value);
            }
        }

        public string Bearer
        {
            set
            {
                _bearer = value;
                _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearer);
            }
        }

        public async Task<T?> GetAsync<T>(string url, Dictionary<string, object> parameters, object? bodyContent = null)
        {
            try
            {
                var response = await SendAsync(HttpMethod.Get, url, parameters, bodyContent);
                if (response?.StatusCode == System.Net.HttpStatusCode.OK && response?.Content is not null)
                {
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    if (response.Content.Headers.ContentType?.MediaType != null && response.Content.Headers.ContentType.MediaType.Equals("application/json"))
                    {
                        return JsonConvert.DeserializeObject<T>(jsonContent);
                    }
                    else
                    {
                        Log.Warning($"Received response type '{response.Content.Headers.ContentType?.MediaType}' from api '{_Client.BaseAddress}{url}'");
                        return (T)Convert.ChangeType(jsonContent, typeof(T));
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error($"GetAsync {_Client.BaseAddress}{url}: {ex.Message}");
            }
            return default(T);
        }
        public async Task<T?> PostAsync<T>(string url, Dictionary<string, object> parameters, object? bodyContent = null)
        {
            try
            {
                var response = await SendAsync(HttpMethod.Post, url, parameters, bodyContent);

                if (response?.StatusCode == System.Net.HttpStatusCode.OK && response?.Content is not null)
                {
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    if (response.Content.Headers.ContentType?.MediaType != null && response.Content.Headers.ContentType.MediaType.Equals("application/json"))
                    {
                        return JsonConvert.DeserializeObject<T>(jsonContent);
                    }
                    else
                    {
                        Log.Warning($"Received response type '{response.Content.Headers.ContentType?.MediaType}' from api '{_Client.BaseAddress}{url}'");
                        return (T)Convert.ChangeType(jsonContent, typeof(T));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"PostAsync {_Client.BaseAddress}{url} {ex.Message}");
            }
            return default(T);
        }

        private async Task<HttpResponseMessage?> SendAsync(HttpMethod method, string url, Dictionary<string, object> parameters, object? bodyContent = null)
        {
            try
            {
                if (parameters.Count > 0)
                {
                    StringBuilder parameterString = new StringBuilder();
                    parameterString.Append('?');
                    foreach (var item in parameters)
                    {
                        parameterString.Append($"{item.Key}={item.Value}&");
                    }
                    url = url + parameterString.ToString().Trim('&');
                }

                var request = new HttpRequestMessage(method, url);
                if (bodyContent is not null)
                {
                    request.Content = JsonContent.Create(bodyContent);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }
                else request.Content = new StringContent("", Encoding.UTF8, "application/json");

                if (string.IsNullOrEmpty(_Client?.BaseAddress?.OriginalString))
                {
                    Log.Error($"Base address of the http client is empty!");
                }

                var result = await _Client.SendAsync(request);
                Log.Information($"API {method} request: {_Client.BaseAddress}{url}. Response code: {result.StatusCode}");

                return result;
            }
            catch (Exception e)
            {
                Log.Warning($"{method} request failed to {_Client.BaseAddress}{url}. Error: {e.Message}");
                return null;
            }
        }

    }
}
