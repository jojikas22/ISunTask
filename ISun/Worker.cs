using Contracts.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace ISun;

public class Worker : BackgroundService
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger _logger;

    public Worker(IWeatherService weatherService, ILogger<Worker> logger, IConfiguration configuration)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            string[] citiesArgs = Environment.GetCommandLineArgs();

            if (!citiesArgs.Contains("--cities"))
            {
                _logger.LogError($"Start application with args: --cities city1, city2, ..., cityN.");
                return;
            }

            citiesArgs = citiesArgs[2..];

            await _weatherService.InitializeAutomatedFetch(
                citiesArgs.Select(s => new CityDto { City = s.Trim(',') }).ToList(),
                stoppingToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}