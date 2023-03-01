using Contracts.V1;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Persistence.Api;
using System.Net;
using Services.Interfaces;

namespace ISun.Tests.Apis;

[TestFixture]
public class WeatherISunApiTests
{
    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private Mock<IAuthService> _authServiceMock;
    private Mock<ILogger<WeatherISunApi>> _logger;

    [SetUp]
    public void Setup()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _authServiceMock = new Mock<IAuthService>();
        _authServiceMock.Setup(s => s.GetToken(It.IsAny<CancellationToken>())).ReturnsAsync(() => "token123");
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        httpClient.BaseAddress = new Uri("http://testAddress.com");
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
        _logger = new Mock<ILogger<WeatherISunApi>>();
    }

    [Test]
    public async Task GetCities_ReturnsListOfCityDto()
    {
        // Arrange
        var citiesFromResponse = new List<string> { "Kaunas", "Vilnius" }.ToArray();
        var expectedCities = new List<CityDto>
        {
            new CityDto { City = "Kaunas" },
            new CityDto { City = "Vilnius" }
        };
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(citiesFromResponse));
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);
        var weatherISunApi = new WeatherISunApi(_httpClientFactoryMock.Object, _authServiceMock.Object, _logger.Object);

        // Act
        var actualCityDtos = await weatherISunApi.GetCities(CancellationToken.None);

        // Assert
        _authServiceMock.Verify(s => s.GetToken(It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsNotNull(actualCityDtos);
        Assert.That(actualCityDtos?.Count, Is.EqualTo(expectedCities.Count));
    }

    [Test]
    public async Task GetCityWeathers_ReturnsCityWeatherDto()
    {
        // Arrange
        var expectedCityWeather = new CityWeatherDto
        {
            City = "Vilnius", Precipitation = 5, WindSpeed = 1, Temperature = 25, Summary = "Good enough"
        };
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(expectedCityWeather));
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        var weatherISunApi = new WeatherISunApi(_httpClientFactoryMock.Object, _authServiceMock.Object, _logger.Object);

        // Act
        var actualCityWeather = await weatherISunApi.GetCityWeathers("Vilnius", CancellationToken.None);

        // Assert
        _authServiceMock.Verify(s=>s.GetToken(It.IsAny<CancellationToken>()),Times.Once);
        Assert.IsNotNull(actualCityWeather);
        Assert.That(actualCityWeather!.City, Is.EqualTo(expectedCityWeather.City));
        Assert.That(actualCityWeather.Temperature, Is.EqualTo(expectedCityWeather.Temperature));
    }

    [Test]
    public async Task GetCityWeathers_ReturnsNull()
    {
        // Arrange
        var apiReturn = new CityWeatherDtoNotFound()
        {
            Title = "NotFound"
        };
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(apiReturn));
        httpResponseMessage.StatusCode = HttpStatusCode.NotFound;
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        var weatherISunApi = new WeatherISunApi(_httpClientFactoryMock.Object, _authServiceMock.Object, _logger.Object);

        // Act
        var actualCityWeather = await weatherISunApi.GetCityWeathers("Vilnius2", CancellationToken.None);

        // Assert
        _authServiceMock.Verify(s => s.GetToken(It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsNull(actualCityWeather);
    }
}