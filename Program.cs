// Existing using statements
using rotaryproject.Components;
using Microsoft.EntityFrameworkCore;
using rotaryproject.Data;
using rotaryproject.Data.Models; // Ensure ApplicationUser is in this namespace or add its namespace
using rotaryproject.Services;

// Using statements for ASP.NET Core Identity
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Connection String (you already have this)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2. DbContext for your application data AND Identity data
// Your RotaryEngineDbContext should now inherit from IdentityDbContext<ApplicationUser>
builder.Services.AddDbContext<RotaryEngineDbContext>(options =>
    options.UseSqlServer(connectionString)
           .LogTo(Console.WriteLine, LogLevel.Information) 
           .EnableSensitiveDataLogging());

// 3. Configure ASP.NET Core Identity Services
// This uses ApplicationUser as your user type and RotaryEngineDbContext for storage.
builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    // Set to false for simpler setup during development (no email confirmation needed).
    // For production, you'd typically set this to true and configure an email sender.
    options.SignIn.RequireConfirmedAccount = false; 

    // You can customize password policies here if desired:
    // options.Password.RequireDigit = true;
    // options.Password.RequireLowercase = true;
    // options.Password.RequireUppercase = true;
    // options.Password.RequireNonAlphanumeric = false;
    // options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<RotaryEngineDbContext>();

// 4. Blazor services (you already have this)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options => {
        options.DetailedErrors = true; // <<< ADD THIS
    });

// 5. Your custom services (you already have this)
builder.Services.AddScoped<rotaryproject.Services.EngineBuildStateService>();
builder.Services.AddCascadingAuthenticationState();

// 6. Add services for Razor Pages (needed for the default Identity UI which uses Razor Pages)
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// app.UseHttpsRedirection(); // You have this commented out, keeping it that way. Enable if you want to enforce HTTPS.

app.UseStaticFiles(); // Serves static files from wwwroot (important for Identity UI's CSS/JS if not handled by MapStaticAssets for RCLs)

app.UseRouting(); // Marks the position in the middleware pipeline where routing decisions are made.

// 7. Add Authentication and Authorization middleware
// IMPORTANT: UseAuthentication() must come BEFORE UseAuthorization().
app.UseAuthentication(); // Enables authentication capabilities (e.g., reading cookies)
app.UseAuthorization();  // Enables authorization capabilities (e.g., restricting access)

app.UseAntiforgery(); // You already have this, good for protecting against CSRF.

// app.MapStaticAssets(); // .NET 9 specific for static assets from wwwroot and RCLs. Should work with UseStaticFiles.

// 8. Map Blazor components (you already have this)
// Ensure 'App' is correctly namespaced if 'rotaryproject.Components.App' is its full name
app.MapRazorComponents<App>() 
    .AddInteractiveServerRenderMode();

// 9. Map Razor Pages (needed for the scaffolded Identity UI routes like /Account/Login, /Account/Register)
app.MapRazorPages();

app.Run();