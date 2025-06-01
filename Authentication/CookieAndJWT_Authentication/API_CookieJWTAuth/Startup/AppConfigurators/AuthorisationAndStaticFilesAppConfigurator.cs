using Microsoft.Extensions.FileProviders;

namespace API_CookieJWTAuth.Startup.AppConfigurators;

public class AuthorisationAndStaticFilesAppConfigurator
{
    public static void Configure(WebApplication app, WebApplicationBuilder builder)
    {
        // Serve index.html as the default document
        app.UseDefaultFiles(new DefaultFilesOptions
        {
            DefaultFileNames = new List<string> { "index.html" },
            FileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.ContentRootPath, "dist"))
        });
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
    }
}