using BlazorSSR_IdentityCookie.AppConfiguration;

var builder = WebApplication.CreateBuilder(args);
ServiceConfigurator.Configure(builder);
AuthenticationConfigurator.Configure(builder);

var app = builder.Build();
AppConfigurator.Configure(app);
ApiConfigurator.Configure(app);
app.Run();
