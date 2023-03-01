using System.Net.Http.Json;
using System.Text;
using Contracts.V1;
using Services.Interfaces;

namespace Services;

public  class AuthService : IAuthService
{
    private static string? _token;
    private readonly IHttpClientFactory _httpFactory;
    public AuthService(IHttpClientFactory httpClientFactory)
    {
        _httpFactory = httpClientFactory;
    }
    public async Task<string> GetToken(CancellationToken ctx)
    {
        if (_token == null)
        {
            var client = _httpFactory.CreateClient();
            var requestBody = "{\r\n    \"username\":\"isun\",\r\n    \"password\":\"passwrod\"\r\n}";
            if (client.BaseAddress is null)
                throw new Exception("Check http client configuration");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(client.BaseAddress, "/api/authorize"))
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json"),
            };
            var response = await client.SendAsync(requestMessage, ctx);
            var token = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: ctx);
            _token = token?.Token!;
            return _token;
        }

        return _token;
    }
}