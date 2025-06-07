# Areas in ASP.NET

[https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/areas?view=aspnetcore-9.0](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/areas?view=aspnetcore-9.0)

A way to seperate controllers, views, controllers with views etc into smaller modules.

## Requirements

1. Areas Folder structure
2. Areas attribute on route
3. Area route mapping in startup / program.cs

### Structure

📂 MyAspNetApp  
├── 📂 Areas  
│ ├── 📂 Customer  
│ │ ├── 📂 Controllers  
│ │ │ ├── 📄 HomeController.cs  
│ │ ├── 📂 Views  
│ │ │ ├── 📂 Home  
│ │ │ │ ├── 📄 Index.cshtml  
├── 📂 Controllers  
│ ├── 📄 HomeController.cs  
├── 📂 Views  
│ ├── 📂 Home  
│ │ ├── 📄 Index.cshtml

### Area attribute

\[Area(“Admin”)\]

- apply to target controllers in Area directory

### Area route configuration

```
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
```

## A way to prevent manual prefix routes

Using areas is functionally similar to manually prefixing routes with `"Admin"` or `"Customer"`, but **areas provide built-in structure and organization** rather than requiring manual route definitions for every controller.

With conventional routing, you might explicitly define a pattern like:

```
app.MapControllerRoute(
    name: "adminRoute",
    pattern: "Admin/{controller}/{action}/{id?}");
```

This forces every admin-related controller to have `"Admin"` in its route manually.

However, areas provide a more **scalable** approach:

1. Controllers inside an area automatically inherit the area name as part of their route.
2. The `[Area]` attribute ensures proper scoping and prevents conflicts.
3. Areas keep code neatly organized into `Areas/Admin/Controllers/HomeController.cs`, etc.

So while it's similar to manually defining `"Admin/Home"` and `"Customer/Home"`, using **areas provides better maintainability**, avoiding hardcoded route prefixes and offering a more structured way to manage large applications.

Are you considering areas for an app you're working on, or just exploring routing strategies?