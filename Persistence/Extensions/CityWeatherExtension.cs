using Contracts.V1;
using Domain.Entities;

namespace Persistence.Extensions;

public static class CityWeatherExtension
{
    public static CityWeather MapToCityWeather(this CityWeatherDto cityWeather)
    {
        return new CityWeather
        {
            City = cityWeather.City,
            Temperature = cityWeather.Temperature,
            Summary = cityWeather.Summary,
            Precipitation = cityWeather.Precipitation,
            WindSpeed = cityWeather.WindSpeed
        };
    }
}