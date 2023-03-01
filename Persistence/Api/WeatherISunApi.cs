using Contracts.V1;
using Domain.Apis.Base;
using Newtonsoft.Json;
using Persistence.Api.Base;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace Persistence.Api;

public class WeatherISunApi : BaseApi, IWeatherApi
{
    private readonly ILogger<WeatherISunApi> _logger;

    public WeatherISunApi(IHttpClientFactory httpClientFactory, IAuthService authService,
        ILogger<WeatherISunApi> logger) : base(httpClientFactory, authService)
    {
        _logger = logger;
    }

    #region Cities

    public async Task<List<CityDto>?> GetCities(CancellationToken ctx)
    {
        var response = await Request("/api/cities", HttpMethod.Get, ctx);
        response.EnsureSuccessStatusCode();
        var cities =
            JsonConvert.DeserializeObject<List<string>>(await response.Content.ReadAsStringAsync(ctx))!.Select(c =>
                new CityDto { City = c });
        return cities.ToList();
    }

    #endregion

    #region Weathers

    public async Task<CityWeatherDto?> GetCityWeathers(string city, CancellationToken ctx)
    {
        var pairs = new Dictionary<string, string>
        {
            {
                "city", city
            }
        };
        var response = await Request("/api/weathers/{city}", HttpMethod.Get, ctx, pairs: pairs);
        if (!response.IsSuccessStatusCode)
        {
            var badResponse =
                JsonConvert.DeserializeObject<CityWeatherDtoNotFound>(
                    await response.Content.ReadAsStringAsync(ctx));
            _logger.LogError($"GetCityWeathers: {city} {badResponse!.Title}");
            return null;
        }

        var weather = JsonConvert.DeserializeObject<CityWeatherDto>(await response.Content.ReadAsStringAsync(ctx));
        return weather;
    }

    #endregion

}