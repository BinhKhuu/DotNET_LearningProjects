using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

namespace BlazorSSR_IdentityCookie.AppConfiguration;

public class AuthenticationConfigurator
{
    public static void Configure(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
        
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromSeconds(1);
            options.SlidingExpiration = false;
            options.LogoutPath = "/Account/Logout";
        });

        builder.Services.AddAuthorizationCore();
    }
}