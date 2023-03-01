using Contracts.V1;
using Domain.Repositories;
using Newtonsoft.Json;
using Persistence.Extensions;

namespace Persistence.Repository;

public class JsonFileRepository : ICityWeatherRepository
{
    private readonly string _path;

    public JsonFileRepository()
    {
        _path = AppDomain.CurrentDomain.BaseDirectory + "Storage";
        Directory.CreateDirectory(_path);
        _path += "/JsonSave.json";
        var file = File.Create(_path);
        file.Dispose();
    }

    public async Task SaveCitiesWeather(List<CityWeatherDto> citiesWeather, CancellationToken ctx)
    {
        var itemsForSave = citiesWeather.Select(s => s.MapToCityWeather());
        var json = JsonConvert.SerializeObject(itemsForSave);
        await File.AppendAllTextAsync(_path, json + Environment.NewLine, ctx);
    }

    public async Task SaveCityWeather(CityWeatherDto cityWeather, CancellationToken ctx)
    {
        var json = JsonConvert.SerializeObject(cityWeather);
        await File.AppendAllTextAsync(_path, json + Environment.NewLine, ctx);
    }
}