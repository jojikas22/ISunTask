using Domain.Repositories;
using ISun;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence.Api;
using Persistence.Repository;
using Services;
using Services.Interfaces;
using Domain.Apis.Base;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

var host = CreateHostBuilder().Build();

#region Setup
// var path = AppDomain.CurrentDomain.BaseDirectory + "Logs";
// Directory.CreateDirectory(path);
// var tracePath = Path.Join(path, $"Log_{DateTime.Now.ToString("yyyyMMdd-HHmm")}.txt");
// Trace.Listeners.Add(new TextWriterTraceListener(tracePath));
// Trace.AutoFlush = true;
#endregion

//Start
await host.RunAsync();

static IHostBuilder CreateHostBuilder() => Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddLogging(b =>
        {
            b.SetMinimumLevel(LogLevel.Trace);
            b.AddNLog(new NLogProviderOptions
            {
                CaptureMessageProperties = true,
                CaptureMessageTemplates = true
            });
        });
        services.AddHostedService<Worker>();
        services.Configure<HostOptions>(hostOptions =>
        {
            hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
        });
        services.AddHttpClient<IWeatherApi>("", client =>
        {
            client.BaseAddress = new Uri("https://weather-api.isun.ch");
            client.Timeout = TimeSpan.FromSeconds(5);
        }).SetHandlerLifetime(TimeSpan.FromMinutes(10));
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IWeatherApi, WeatherISunApi>();
        services.AddTransient<IWeatherService, WeatherService>();
        services.AddTransient<ICityWeatherRepository, JsonFileRepository>();
    });