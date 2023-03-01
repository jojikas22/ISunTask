using Contracts.V1;

namespace Domain.Apis.Base;

public interface IWeatherApi
{
    Task<List<CityDto>?> GetCities(CancellationToken ctx);
    Task<CityWeatherDto?> GetCityWeathers(string city, CancellationToken ctx);
}