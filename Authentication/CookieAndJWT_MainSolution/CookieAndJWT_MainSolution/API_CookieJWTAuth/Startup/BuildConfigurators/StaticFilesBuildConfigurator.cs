namespace API_CookieJWTAuth.Startup.BuildConfigurators;

public class StaticFilesBuildConfigurator
{
    public static void Configure(WebApplicationBuilder builder)
    {
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

    }
}