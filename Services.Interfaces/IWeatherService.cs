using Contracts.V1;

namespace Services.Interfaces;

public interface IWeatherService
{
    Task<List<CityDto>?> GetCitiesListAsync(CancellationToken ctx);
    Task<CityWeatherDto?> GetCityWeatherAsync(CityDto city,CancellationToken ctx);
    Task InitializeAutomatedFetch(List<CityDto> cities, CancellationToken ctx, bool runOnce = false);
}