using BlazorSSR_IdentityCookie.Middleware;

namespace BlazorSSR_IdentityCookie.AppConfiguration;

public class HttpClientConfigurator
{
    public static void Configure(WebApplicationBuilder builder)
    {
        
        builder.Services.AddHttpClient(
                "AuthClient",
                opt => opt.BaseAddress = new Uri("http://localhost:5284"))
            .AddHttpMessageHandler<CookieHandler>();
        builder.Services.AddHttpClient(
            "UnAuthClient",
            opt => opt.BaseAddress = new Uri("http://localhost:5284"));
        builder.Services.AddHttpClient(
                "WebApiAuthClient",
                opt => opt.BaseAddress = new Uri("http://localhost:5115"))
            .AddHttpMessageHandler<CookieHandler>();
        builder.Services.AddHttpClient(
            "WebApiUnAuthClient",
            opt => opt.BaseAddress = new Uri("http://localhost:5115"));



    }
}