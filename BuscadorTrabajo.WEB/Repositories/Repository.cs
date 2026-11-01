using System.Net;
using System.Text;
using System.Text.Json;

namespace BuscadorTrabajo.WEB.Repositories
{
    // Único lugar donde la aplicación web habla con el API.
    // Las páginas no usan HttpClient directamente: llaman a este repositorio.
    public class Repository : IRepository
    {
        private readonly HttpClient _httpClient;

        private JsonSerializerOptions _jsonDefaultOptions => new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public Repository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: consultar datos
        public async Task<HttpResponseWrapper<T>> GetAsync<T>(string url)
        {
            var responseHttp = await EnviarAsync(() => _httpClient.GetAsync(url));
            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await UnserializeAnswer<T>(responseHttp, _jsonDefaultOptions);
                return new HttpResponseWrapper<T>(response, false, responseHttp);
            }

            return new HttpResponseWrapper<T>(default, true, responseHttp);
        }

        // POST: crear un registro
        public async Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T model)
        {
            var messageContent = ToJson(model);
            var responseHttp = await EnviarAsync(() => _httpClient.PostAsync(url, messageContent));
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
        }

        public async Task<HttpResponseWrapper<TResponse>> PostAsync<T, TResponse>(string url, T model)
        {
            var messageContent = ToJson(model);
            var responseHttp = await EnviarAsync(() => _httpClient.PostAsync(url, messageContent));
            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await UnserializeAnswer<TResponse>(responseHttp, _jsonDefaultOptions);
                return new HttpResponseWrapper<TResponse>(response, false, responseHttp);
            }

            return new HttpResponseWrapper<TResponse>(default, true, responseHttp);
        }

        // PUT: editar un registro
        public async Task<HttpResponseWrapper<object>> PutAsync<T>(string url, T model)
        {
            var messageContent = ToJson(model);
            var responseHttp = await EnviarAsync(() => _httpClient.PutAsync(url, messageContent));
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
        }

        public async Task<HttpResponseWrapper<TResponse>> PutAsync<T, TResponse>(string url, T model)
        {
            var messageContent = ToJson(model);
            var responseHttp = await EnviarAsync(() => _httpClient.PutAsync(url, messageContent));
            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await UnserializeAnswer<TResponse>(responseHttp, _jsonDefaultOptions);
                return new HttpResponseWrapper<TResponse>(response, false, responseHttp);
            }

            return new HttpResponseWrapper<TResponse>(default, true, responseHttp);
        }

        // DELETE: borrar un registro
        public async Task<HttpResponseWrapper<object>> DeleteAsync(string url)
        {
            var responseHttp = await EnviarAsync(() => _httpClient.DeleteAsync(url));
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
        }

        // Hace la petición. Si el API está apagado o no se puede conectar, en lugar de que la
        // aplicación se caiga con un error, devuelve una respuesta 503 con un mensaje claro.
        private static async Task<HttpResponseMessage> EnviarAsync(Func<Task<HttpResponseMessage>> peticion)
        {
            try
            {
                return await peticion();
            }
            catch (HttpRequestException)
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent("No se pudo conectar con el servidor. Verifica que el API esté encendido e inténtalo de nuevo.")
                };
            }
        }

        // Convierte un objeto de C# en JSON para enviarlo al API
        private static StringContent ToJson<T>(T model)
        {
            var messageJson = JsonSerializer.Serialize(model);
            return new StringContent(messageJson, Encoding.UTF8, "application/json");
        }

        // Convierte el JSON que responde el API en un objeto de C#
        private async Task<T> UnserializeAnswer<T>(HttpResponseMessage httpResponse, JsonSerializerOptions jsonSerializerOptions)
        {
            var respuestaString = await httpResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(respuestaString, jsonSerializerOptions);
        }
    }
}
