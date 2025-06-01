
# Project
Learning about Cookie and Jwt Authentication and authorisation
Project uses Cookie Authentication to protect static assets and JwtBearer Authorization on the api's
SPA app is served from /dist folder
Currently not configured for localhost proxying to SPA 
Uses MSAL to log user in and attach bearer token to http request


## WebAPI packages

```
Microsoft.AspNetCore.Authentication.Cookies;
Microsoft.AspNetCore.Authentication.JwtBearer;
Microsoft.AspNetCore.Authentication.OpenIdConnect;
Microsoft.Identity.Web;

```

## Angular packages
```
npm i @azure/msal-angular @azure/msal-browser
```

## WebAPI Azure Identity settings
App settings for the azure ad configurations

"AzureAd": {
  "Instance": "https://login.microsoftonline.com/",
  "TenantId": "",
  "ClientId": ""
},
"AzureAdCookie": {
  "Instance": "https://login.microsoftonline.com/",
  "TenantId": "",
  "ClientId": "",
  "CallbackPath": "/signin-oidc"
}

## Angular MSAL Azure Identity settings
Protected resources must have an entry otherwise bearer token is not sent to any requests if you are using MSALInterceptor
```
export const msalInterceptorConfig: MsalInterceptorConfiguration = {
    interactionType: InteractionType.Redirect,
    protectedResourceMap: new Map([
        ['http://localhost:5198', ['api://{clientid}/access_as_user']]
    ])
}
```


# WebApi Configuration Authorisation and Authentication setup

Resources

- [https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-9.0#static-file-authorization](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-9.0#static-file-authorization)

## Authentication and Authorisation

- **Authentication** is about verifying **who you are**. It ensures that a user, service, or system is genuine before granting access. Examples include logging in with a password, using biometric scans, or signing in via OAuth.
- **Authorisation** determines **what you can do** once authenticated. It defines the permissions and access levels you have within a system. For instance, a regular user might be able to view certain pages, while an admin has full control over configurations.

## Configure Authentication

```
private static void ConfigureAuthentication(WebApplicationBuilder builder)
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
    
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; 
        //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdCookie"));
}
```

- AddMicrosoftIdentityWebApi configures JwtBearerDefaults for you, the middleware and other validators based on the application registration settings.
-   configuration includes auth flow like PKCE support enabling the SPA application setting on the app reg.
-   designed for Web Apis
- AddMicrosoftIdentityWebApp configures authentication settings for you but with web apps in mind. Under the hood middleware and apis are configured ( like /signin-oidc ) and enables PKCE support for the SPA application setting on the app reg.
-   specifying a cookie for Authentication
- Using both seems to work I think its because
-   AddMicrosoftIdentityWebApi adds middleware, validators and other things to handle jwt bearers and it doesn’t overlap with AddMicrosoftIdentityWebApp which does the same thing but using cookie and targeting web apps not web apis
  
  -   there could be an issue if you create a custom signin-oidc
- **No Direct Conflict, But Potential Ordering Issues**
-   The **default authentication scheme** matters. If a request is processed without an explicit scheme, the app will use the default (`options.DefaultAuthenticateScheme`).
-   If an API endpoint expects a JWT but defaults to cookies, it may not authenticate correctly.
-   Authorization policies should clearly define which authentication schemes apply to specific request types.

## Configure Authorisation

```
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Bearer", policy =>
        policy.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));
});
```

- Add Authorization defines “Bearer” policy that requires \[Authorized\] resources to require a JwtBearer token
-   this is an Authorization configuration
-   there are other tags that can be used like RequireAuthorization
-   typically put this on the api controllers

## Configure Static Asset Authentication

```
builder.Services.AddAuthorizationCore();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
```

- see static assets reference [https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-9.0#static-file-authorization](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-9.0#static-file-authorization)

## Alternative Authentication

```
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
```

- configuring JWTBearer and using AddMicrosoftIdentityWebApp
- could also reverse and use AddMicrosoftIdentityWebApi and configure cookie
-   cookie configuration requires Web application from app reg and a secret value

```
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
        options.Authority = $"https://login.microsoftonline.com/{tenantId}";
        options.ClientId = "{clientId}";
        options.ClientSecret = "secretValueFromAppReg"; // Important for web apps!
        options.ResponseType = "code"; // Authorization Code Flow
        options.CallbackPath = "/signin-oidc";
        options.SaveTokens = true;
        options.UsePkce = true; // Enforce PKCE for better security
    });
```

- configuring both OpenIDConnect Cookies and JWT Bearers.
- OpenIDConect requires Web app and different code flow, forcing Pkce does not work
-   and because of this requires client secret


# WebApi configuration Static files setup

- Microsoft.AspNetCore.SpaServices.Extensions

The order in which you setup these is important because middleware is added and executed in the order they are added. Some middleware like UseStaticFiles when executed will terminate the request preventing other middleware from executing so if you add authorisation after it won’t get called.

## Builder Setup SPA static files

```
if (builder.Environment.IsProduction())
{
    // use add spa static files for production to target the /dist folder located in the root (wwwroot)
    var rootPathSetting = "dist";
    var rootPath = string.IsNullOrWhiteSpace(rootPathSetting) ? throw new Exception("Error finding SpaStaticFilesRootPath setting"): rootPathSetting;
    builder.Services.AddSpaStaticFiles(configuration =>
    {
        configuration.RootPath = rootPath;
    });
}
```

## Setup default files fallback

```
// Serve index.html as the default document
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" },
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "dist"))
});

```

## Setup Static files middleware

```
// use Authentication and Authorization before static because static ingores any middleware after
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "dist")),
});
// Fallback to index.html for SPA requests
app.UseEndpoints(endpoints =>
{
    endpoints.MapFallbackToFile("/index.html", new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "dist"))
    });
});

```