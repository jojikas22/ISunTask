namespace Domain.Exceptions;

public sealed class CityNotFoundException: Exception
{
    public CityNotFoundException(string name)
        : base($"An city with name: {name} was not found.")
    {
    }
}