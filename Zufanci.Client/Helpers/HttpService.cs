using System.Text;
using System.Text.Json;

namespace Zufanci.Client.Helpers
{
    /// <summary>
    /// Provides methods for making HTTP requests.
    /// </summary>
    public class HttpService : IHttpService
    {
        private readonly HttpClient httpClient;
        private JsonSerializerOptions defaultJsonSerializerOptions =>
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpService"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used to make HTTP requests.</param>
        public HttpService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Sends a DELETE request to the specified URL.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The task result contains the HTTP response.</returns>
        public async Task<HttpResponseWrapper<object>> Delete(string url)
        {
            var responseHTTP = await httpClient.DeleteAsync(url);
            return new HttpResponseWrapper<object>(null, responseHTTP.IsSuccessStatusCode, responseHTTP);
        }

        /// <summary>
        /// Sends a GET request to the specified URL and deserializes the response body to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response body to.</typeparam>
        /// <param name="url">The URL to send the request to.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The task result contains the HTTP response.</returns>
        public async Task<HttpResponseWrapper<T>> Get<T>(string url)
        {
            HttpResponseMessage responseHTTP = await httpClient.GetAsync(url);

            if (responseHTTP.IsSuccessStatusCode)
            {
                var response = await Deserialize<T>(responseHTTP, defaultJsonSerializerOptions);
                return new HttpResponseWrapper<T>(response, true, responseHTTP);
            }
            else
            {
                return new HttpResponseWrapper<T>(default, false, responseHTTP);
            }
        }

        /// <summary>
        /// Sends a POST request to the specified URL with the specified data.
        /// </summary>
        /// <typeparam name="T">The type of data to send in the request body.</typeparam>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="data">The data to send in the request body.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The task result contains the HTTP response.</returns>
        public async Task<HttpResponseWrapper<object>> Post<T>(string url, T data)
        {
            var dataJson = JsonSerializer.Serialize(data);
            var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, stringContent);
            return new HttpResponseWrapper<object>(null, response.IsSuccessStatusCode, response);
        }

        /// <summary>
        /// Sends a POST request to the specified URL with the specified data and deserializes the response body to the specified type.
        /// </summary>
        /// <typeparam name="T">The type of data to send in the request body.</typeparam>
        /// <typeparam name="TResponse">The type to deserialize the response body to.</typeparam>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="data">The data to send in the request body.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The task result contains the HTTP response.</returns>
        public async Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T data)
        {
            var dataJson = JsonSerializer.Serialize(data);
            var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, stringContent);
            if (response.IsSuccessStatusCode)
            {
                var responseDeserialized = await Deserialize<TResponse>(response, defaultJsonSerializerOptions);
                return new HttpResponseWrapper<TResponse>(responseDeserialized, true, response);
            }
            else
            {
                return new HttpResponseWrapper<TResponse>(default, false, response);
            }
        }

        /// <summary>
        /// Sends a PUT request to the specified URL with the specified data.
        /// </summary>
        /// <typeparam name="T">The type of data to send in the request body.</typeparam>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="data">The data to send in the request body.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The task result contains the HTTP response.</returns>
        public async Task<HttpResponseWrapper<object>> Put<T>(string url, T data)
        {
            var dataJson = JsonSerializer.Serialize(data);
            var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync(url, stringContent);
            return new HttpResponseWrapper<object>(null, response.IsSuccessStatusCode, response);
        }

        private async Task<T> Deserialize<T>(HttpResponseMessage httpResponse, JsonSerializerOptions options)
        {
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseString, options);
        }
    }
}
