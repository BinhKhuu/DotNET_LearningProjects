using API_CookieJWTAuth.Controllers;
using API_CookieJWTAuth.Startup.AppConfigurators;
using API_CookieJWTAuth.Startup.BuildConfigurators;

var builder = WebApplication.CreateBuilder(args);
AuthenticationBuildConfigurator.Configure(builder);
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.UseRouting();
AuthorisationAndStaticFilesAppConfigurator.Configure(app,builder);
app.MapWeatherforecastEndPoints();
app.Run();
