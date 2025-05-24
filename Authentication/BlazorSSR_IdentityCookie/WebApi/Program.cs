using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

// configure openId evens
builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Events = new OpenIdConnectEvents
    {
        OnAuthenticationFailed = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.HandleResponse(); // Stops default redirect behavior
            return Task.CompletedTask;
        },
        OnAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.HandleResponse(); // Stops default redirect behavior
            return Task.CompletedTask;
        }
    };


});
builder.Services.AddAuthorizationBuilder();

// enable cores and cookie sharing
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:5284")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// configure cookies for sharing on subdomains
// attempetd to use .localhost subdomain but doesn't work
// for real subdomains like .mydomain this should work
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.Domain = ".localhost";
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/api/weatherforecast", () =>
    {
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
    .RequireAuthorization();

app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}