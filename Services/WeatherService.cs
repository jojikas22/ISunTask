using Contracts.V1;
using Domain.Apis.Base;
using Domain.Exceptions;
using Domain.Repositories;
using Services.Interfaces;
using Microsoft.Extensions.Logging;
namespace Services;

public class WeatherService : IWeatherService
{
    private readonly ILogger _logger;
    private readonly IWeatherApi _weatherISunApi;
    private readonly ICityWeatherRepository _weatherRepository;

    public WeatherService(ILogger<WeatherService> logger, IWeatherApi weatherISunApi, ICityWeatherRepository weatherRepository)
    {
        _logger = logger;
        _weatherISunApi = weatherISunApi;
        _weatherRepository = weatherRepository;
    }

    public async Task<List<CityDto>?> GetCitiesListAsync(CancellationToken ctx)
    {
        return await _weatherISunApi.GetCities(ctx);
    }

    public async Task<CityWeatherDto?> GetCityWeatherAsync(CityDto city, CancellationToken ctx)
    {
        return await _weatherISunApi.GetCityWeathers(city.City!, ctx);
    }

    public async Task InitializeAutomatedFetch(List<CityDto> cities, CancellationToken ctx, bool runOnce = false)
    {
        var allAvailableCities = ((await GetCitiesListAsync(ctx))!).Select(s=>s.City).ToList();
        var notExistingOnes = cities.Where(c => !allAvailableCities.Contains(c.City))
            .ToList();

        foreach (var notExistingOne in notExistingOnes)
        {
            cities!.Remove(notExistingOne);
            throw new CityNotFoundException(notExistingOne.City!);
        }

        _logger.LogInformation($"Initializing weather reading for cities");
        var shouldRuneOnce = false;
        while (!shouldRuneOnce)
        {
            var citiesToSave = new List<CityWeatherDto>();
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 5
            };
            await Parallel.ForEachAsync(cities!, options, async (city,ct) =>
            {
                var cityWeather = await GetCityWeatherAsync(city, ct);
                citiesToSave.Add(cityWeather!);
                _logger.LogInformation(cityWeather!.ToString());
            });
            await _weatherRepository.SaveCitiesWeather(citiesToSave,ctx);
            if (runOnce)
            {
                shouldRuneOnce = true;
                continue;
            }
            await Task.Delay(15000, ctx);
        }
    }
}