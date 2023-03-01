using Contracts.V1;

namespace Domain.Repositories;

public interface ICityWeatherRepository
{
    Task SaveCitiesWeather(List<CityWeatherDto> cityWeather, CancellationToken ctx);
    Task SaveCityWeather(CityWeatherDto cityWeather, CancellationToken ctx);
}