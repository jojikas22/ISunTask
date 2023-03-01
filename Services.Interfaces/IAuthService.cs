namespace Services.Interfaces;

public interface IAuthService
{
    Task<string> GetToken(CancellationToken ctx);
}