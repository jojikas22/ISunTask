using Contracts.V1;
using Domain.Apis.Base;
using Domain.Exceptions;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Services;

namespace ISun.Tests.Services;

[TestFixture]
public class WeatherServiceTest
{
    private Mock<IWeatherApi> _weatherApiMock;
    private Mock<ICityWeatherRepository> _cityWeatherRepositoryMock;
    private Mock<ILogger<WeatherService>> _loggerMock;
    private WeatherService _weatherService;

    [SetUp]
    public void Setup()
    {
        _weatherApiMock = new Mock<IWeatherApi>();
        _cityWeatherRepositoryMock = new Mock<ICityWeatherRepository>();
        _loggerMock = new Mock<ILogger<WeatherService>>();
        _weatherService = new WeatherService(_loggerMock.Object, _weatherApiMock.Object,
            _cityWeatherRepositoryMock.Object);

    }

    [Test]
    public Task InitializeAutomatedFetch_ShouldThrowException_When_CityIsNotAvailable()
    {
        // Arrange
        var cities = new List<CityDto>
        {
            new() { City = "Vilnius" },
            new() { City = "Kaunas" },
            new() { City = "NotExistingOne" }
        };

        _weatherApiMock.Setup(x => x.GetCities(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CityDto> { new() { City = "Vilnius" }, new() { City = "Kaunas" } });
        
        // Act + Assert
        Assert.ThrowsAsync<CityNotFoundException>(async () => await _weatherService.InitializeAutomatedFetch(cities, It.IsAny<CancellationToken>()));
        return Task.CompletedTask;
    }

    [Test]
    public async Task InitializeAutomatedFetch_ShouldSaveCityWeather_UsingRepository()
    {
        // Arrange
        var cities = new List<CityDto>
        {
            new() { City = "Vilnius" },
            new() { City = "Kaunas" }
        };

        var cityWeather = new CityWeatherDto
        {
            City = "Vilnius",
            Temperature = -1,
            WindSpeed = 2
        };
        _weatherApiMock.Setup(x => x.GetCities(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cities);
        _weatherApiMock.Setup(x => x.GetCityWeathers(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cityWeather);
        _cityWeatherRepositoryMock.Setup(x => x.SaveCitiesWeather(It.IsAny<List<CityWeatherDto>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _weatherService.InitializeAutomatedFetch(cities, It.IsAny<CancellationToken>(), true);

        // Assert
        _cityWeatherRepositoryMock.Verify(x => x.SaveCitiesWeather(It.IsAny<List<CityWeatherDto>>(), It.IsAny<CancellationToken>()), Times.Once);
    }


    [Test]

    public async Task InitializeAutomatedFetch_ShouldFetchWeatherForAllCities()
    {
        // Arrange
        var citiesToCheck = new List<CityDto> { new CityDto { City = "Vilnius" }, new CityDto { City = "Kaunas" } };
        var allAvailableCities = new List<CityDto> { new CityDto { City = "Vilnius" }, new CityDto { City = "Kaunas" }, new CityDto { City = "Utena" } };
        var weather1 = new CityWeatherDto { City = "Vilnius", Temperature = 20 };
        var weather2 = new CityWeatherDto { City = "Kaunas", Temperature = 15 };

        _weatherApiMock.Setup(x => x.GetCities(It.IsAny<CancellationToken>())).ReturnsAsync(allAvailableCities);

        _weatherApiMock.SetupSequence(x => x.GetCityWeathers(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(weather1)
            .ReturnsAsync(weather2);
        _cityWeatherRepositoryMock.Setup(x => x.SaveCitiesWeather(It.IsAny<List<CityWeatherDto>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var service = new WeatherService(_loggerMock.Object, _weatherApiMock.Object, _cityWeatherRepositoryMock.Object);

        // Act
        await service.InitializeAutomatedFetch(citiesToCheck, It.IsAny<CancellationToken>(),true);

        // Assert
        _weatherApiMock.Verify(x => x.GetCityWeathers("Vilnius", It.IsAny<CancellationToken>()), Times.Once);
        _weatherApiMock.Verify(x => x.GetCityWeathers("Kaunas", It.IsAny<CancellationToken>()), Times.Once);
        _cityWeatherRepositoryMock.Verify(x => x.SaveCitiesWeather(It.IsAny<List<CityWeatherDto>>(), It.IsAny<CancellationToken>()), Times.Once);

    }
}