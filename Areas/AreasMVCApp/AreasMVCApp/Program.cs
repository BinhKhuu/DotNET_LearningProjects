var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/HomeController/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// maps /customer
app.MapControllerRoute(
    name: "customer_home",
    pattern: "customer",
    defaults: new { area = "Customer",controller = "Home", action = "Index" });

// maps /admin
app.MapControllerRoute(
    name: "admin_home",
    pattern: "admin",
    defaults: new { area = "Admin",controller = "Home", action = "Index" });

// maps all area routes
app.MapControllerRoute(
        name: "areas", 
        pattern: "{area}/{controller}/{action}/{id?}");

// maps main controller outside of area
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=HomeController}/{action=Index}/{id?}")
    .WithStaticAssets();

// fallback map to main home controller outside of area
app.MapControllerRoute(
    name: "root",
    pattern: "",
    defaults: new { controller = "Home", action = "Index" });


app.Run();