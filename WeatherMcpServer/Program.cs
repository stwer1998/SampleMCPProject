using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Tools;
using WeatherMcpServer.Infrastructure;
using WeatherMcpServer.Services;
using WeatherMcpServer.Mappers;

var builder = Host.CreateApplicationBuilder(args);

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

// Add configuration for OpenWeatherMap
builder.Services.Configure<OpenWeatherMapOptions>(builder.Configuration.GetSection("OpenWeatherMap"));
// Add configuration for cache
builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection("Cache"));

// Add memory cache
builder.Services.AddMemoryCache();

// Add AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<WeatherProfile>());

// Add HttpClient
builder.Services.AddHttpClient<OpenWeatherMapClient>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));
builder.Services.AddScoped<IOpenWeatherMapClient, OpenWeatherMapClient>();

// Register services
builder.Services.AddScoped<IWeatherService, WeatherService>();

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<RandomNumberTools>()
    .WithTools<WeatherTools>();

await builder.Build().RunAsync();