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
        
        builder.Services.AddAuthorizationCore();
    }
    
    
   
}