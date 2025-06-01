using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace API_CookieJWTAuth.Startup.BuildConfigurators;

public class AuthenticationBuildConfigurator
{

    public static void Configure(WebApplicationBuilder builder)
    {
        // use one of the configuration methods
        ConfigureMSIdentityAuthentication(builder);
        builder.Services.AddAuthentication();
        // Project all resources
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Bearer", policy =>
                policy.RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));
            
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });
    }

    // use .NET MS Identity methods to configure both jwt and cookie authentication and authorization
    private static void ConfigureMSIdentityAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
    
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; 
            })
            .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdCookie"));
    }

    // use .NET MS Identity methods to configure cookie authentication. Manually configure jwt
    private static void ConfigureMsIdentityCookieAndJwtBearer(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "https://login.microsoftonline.com/{tenantId}";
                options.Audience = "api:/{clientId}"; // Ensure correct audience
            })
            .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdCookie"));
    }

    // Manually configure both cookies (OpenIDConnect) and JwtBearer
    private static void ConfigureJwtBearerAndOpenId(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "https://login.microsoftonline.com/{tenantId}";
                options.Audience = "api://{clientId}"; // Ensure correct audience
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "https://login.microsoftonline.com/{tenantId}";
                options.ClientId = "{clientId}";
                options.ClientSecret = "secretValueFromAppReg"; // Important for web apps!
                options.ResponseType = "code"; // Authorization Code Flow
                options.CallbackPath = "/signin-oidc";
                options.SaveTokens = true;
                options.UsePkce = true; // Enforce PKCE for better security
            });
    }
}