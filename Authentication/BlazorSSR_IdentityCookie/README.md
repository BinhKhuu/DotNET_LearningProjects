Blazor Server App using cookies and ms identity to authenticate

# App configuration for blazor and webapi

````
{
    "ConnectionStrings": {
    "DefaultConnection": "DataSource=Data\\app.db;Cache=Shared"
    },
    "Logging": {
        "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "AzureAd": {
        "Instance": "https://login.microsoftonline.com/",
        "TenantId": "",
        "ClientId": "",
        "CallbackPath": "/signin-oidc"
    }
}
````

# Cookie Authentiation
BlazorServer and webapi configured to use cookie authentication via an app registration
- cookies configured to share through subdomain
- does not work when using localhost subdomains

cookies are domain-specific and cannot be shared across completely different domains.

Why Is This Happening?
Cookies are bound to the domain that issued them, meaning:

localhost:5284 and localhost:5115 are treated as separate domains, even though they both start with localhost.

Cookies cannot be shared across different domains unless they share a common root domain (like yourdomain.com and api.yourdomain.com).


# Signal R and cookies
https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-9.0
SignalR connections automatically inherit authentication from the app if the user is logged in, and cookies are used to send access tokens in browser-based apps.
However, if a token expires during the connection's lifetime, the connection continues to work by default. For connections to close when the authentication token expires, the CloseOnAuthenticationExpiration option can be set.