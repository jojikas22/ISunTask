namespace Contracts.V1;

public class CityWeatherDto
{
    public string? City { get; set; }
    public int? Temperature { get; set; }
    public int? Precipitation { get; set; }
    public int? WindSpeed { get; set; }
    public string? Summary { get; set; }

    public override string ToString()
    {
        return $"City: {City}, Temperature: {Temperature}, Precipitation: {Precipitation}, WindSpeed: {WindSpeed}, Summary: {Summary}";
    }
}

public class CityWeatherDtoNotFound
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int Status { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
    public string? AdditionalProp1 { get; set; }
    public string? AdditionalProp2 { get; set; }
    public string? AdditionalProp3 { get; set; }
}