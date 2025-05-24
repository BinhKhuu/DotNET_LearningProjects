using BlazorSSR_IdentityCookie.Components.Account;
using BlazorSSR_IdentityCookie.Data;
using BlazorSSR_IdentityCookie.Middleware;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorSSR_IdentityCookie.AppConfiguration;

public class ServiceConfigurator
{
    public static void Configure(WebApplicationBuilder builder)
    {
        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
        
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
        builder.Services.AddTransient<CookieHandler>();
    }
}