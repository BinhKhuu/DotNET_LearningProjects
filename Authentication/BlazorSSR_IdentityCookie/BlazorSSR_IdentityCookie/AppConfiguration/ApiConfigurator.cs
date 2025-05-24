using Microsoft.AspNetCore.Authentication;

namespace BlazorSSR_IdentityCookie.AppConfiguration;

public class ApiConfigurator
{
    public static void Configure(WebApplication app)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        app.MapGet("api/weatherforecasts", (HttpContext context) =>
            {
                var user = context.User;
                var claims = user.Claims.ToList();
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
            .RequireAuthorization();

        app.MapPost("Account/Logout", (context =>
        {
            context.SignOutAsync();
            return Task.CompletedTask;
        }));
    }
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}