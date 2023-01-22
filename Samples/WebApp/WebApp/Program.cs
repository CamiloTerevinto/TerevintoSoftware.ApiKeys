using TerevintoSoftware.AspNetCore.Authentication.ApiKeys;
using TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Abstractions;
using WebApp;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddSimpleConsole(options => options.IncludeScopes = true);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(setup =>
{
    setup.AddApiKeySupport();
});

builder.Services.AddLocalization(opt => opt.ResourcesPath = "Resources");

builder.Services
    .AddDefaultApiKeyGenerator(new ApiKeyGenerationOptions
    {
        KeyPrefix = "CT-",
        GenerateUrlSafeKeys = true,
        LengthOfKey = 32
    })
    .AddDefaultClaimsPrincipalFactory()
    .AddApiKeys()
    .AddSingleton<IClientsService, InMemoryClientsService>()
    .AddMemoryCache()
    .AddSingleton<IApiKeysCacheService, CacheService>();

var app = builder.Build();

app.UseRequestLocalization(opt =>
{
    opt.AddSupportedCultures("en", "es");
    opt.AddSupportedUICultures("en", "es");
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.RequireAuthorization();

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
