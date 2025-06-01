using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace API_CookieJWTAuth.Controllers;

public static class WeatherforecastController
{
    public static void MapWeatherforecastEndPoints(this IEndpointRouteBuilder app)
    {
        var weatherApi = app.MapGroup("api/weatherforecast");
        
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        weatherApi.MapGet("/", (HttpContext httpContext) =>
            {
                //httpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

                var forecast = Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        (
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)]
                        ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi()
            .RequireAuthorization(JwtBearerDefaults.AuthenticationScheme);
    }
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}