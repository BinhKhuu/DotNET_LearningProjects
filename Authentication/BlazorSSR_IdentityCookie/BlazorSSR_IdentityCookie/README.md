Blazor Server App using cookies and ms identity to authenticate

# App configuration

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
```

