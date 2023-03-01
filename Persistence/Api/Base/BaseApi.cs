using System.Net.Http.Headers;
using System.Text;
using Services.Interfaces;

namespace Persistence.Api.Base;

public abstract class BaseApi
{
    private static string? _token;
    private readonly IHttpClientFactory _httpFactory;
    private readonly IAuthService _authService;
    protected BaseApi(IHttpClientFactory httpClientFactory, IAuthService authService)
    {
        _httpFactory = httpClientFactory;
        _authService = authService;
    }
        
    protected async Task<HttpResponseMessage> Request(string url, HttpMethod method, CancellationToken ctx,
        string json = "", Dictionary<string,string>? pairs = null)
    {
        var client = _httpFactory.CreateClient();
        HttpRequestMessage request;
        if (pairs != null)
        {
            foreach (var pair in pairs)
            {
                url = url.Replace("{"+pair.Key+"}", pair.Value);
            }
            request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(client.BaseAddress!,url)
            };
        }
        else
        {
            request = new HttpRequestMessage
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                Method = method,
                RequestUri = new Uri(client.BaseAddress!, url)
            };
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _authService.GetToken(ctx));
        var response = await client.SendAsync(request, ctx);
        return response;
    }
}