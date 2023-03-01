namespace Domain.Entities;

public class CityWeather
{
    public string? City { get; set; }
    public int? Temperature { get; set; }
    public int? Precipitation { get; set; }
    public int? WindSpeed { get; set; }
    public string? Summary { get; set; }

}