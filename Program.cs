using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using System.Runtime.InteropServices;
using Green_Basket.Data;
using Green_Basket.Models;

// Enable legacy timestamp behavior for Npgsql/PostgreSQL DateTime compatibility
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Support dynamic port from cloud hosts like Render
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// 1. Add Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=GreenBasket_DRP.db";

// Remove accidental newlines or extra whitespace from cloud dashboard inputs
connectionString = connectionString.Replace("\r", "").Replace("\n", "").Trim();

// Fix common typos in connection string keys (e.g. "usernam e" -> "Username")
connectionString = System.Text.RegularExpressions.Regex.Replace(connectionString, @"\busernam\s+e\b", "Username", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
connectionString = System.Text.RegularExpressions.Regex.Replace(connectionString, @"\buser\s+name\b", "Username", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
connectionString = System.Text.RegularExpressions.Regex.Replace(connectionString, @"\bpass\s+word\b", "Password", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

// Fall back to SQLite if running on Linux/Docker and connection string still points to Windows LocalDB
if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
    && connectionString.Contains("localdb", StringComparison.OrdinalIgnoreCase))
{
    connectionString = "Data Source=GreenBasket_DRP.db";
}

bool isPostgres = connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
               || connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)
               || connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase)
               || connectionString.Contains("Server=db.", StringComparison.OrdinalIgnoreCase)
               || connectionString.Contains("supabase", StringComparison.OrdinalIgnoreCase);

if (isPostgres)
{
    // Auto-fix IPv6-only Supabase direct host to IPv4 pooler host for cloud compatibility
    if (connectionString.Contains("db.cxonxfiplynpyrxqdfvy.supabase.co", StringComparison.OrdinalIgnoreCase))
    {
        connectionString = connectionString.Replace("db.cxonxfiplynpyrxqdfvy.supabase.co", "aws-0-ap-southeast-2.pooler.supabase.com");
        if (!connectionString.Contains("postgres.cxonxfiplynpyrxqdfvy", StringComparison.OrdinalIgnoreCase))
        {
            connectionString = connectionString.Replace("postgres:", "postgres.cxonxfiplynpyrxqdfvy:");
            connectionString = connectionString.Replace("Username=postgres;", "Username=postgres.cxonxfiplynpyrxqdfvy;");
        }
    }

    // Convert postgresql:// URI format to Npgsql key-value format if needed
    if (connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
        connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
    {
        try
        {
            var uri = new Uri(connectionString);
            var userInfo = uri.UserInfo.Split(new[] { ':' }, 2);
            var user = Uri.UnescapeDataString(userInfo[0]);
            var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
            var host = uri.Host;
            var dbPort = uri.Port > 0 ? uri.Port : 5432;
            var database = uri.AbsolutePath.TrimStart('/');
            if (string.IsNullOrEmpty(database)) database = "postgres";

            connectionString = $"Host={host};Port={dbPort};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
        }
        catch
        {
            // Use raw string if URI parse fails
        }
    }
    else
    {
        if (!connectionString.Contains("SSL Mode", StringComparison.OrdinalIgnoreCase))
        {
            connectionString += ";SSL Mode=Require;Trust Server Certificate=true;";
        }
    }
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (isPostgres)
    {
        options.UseNpgsql(connectionString);
    }
    else if (connectionString.Contains(".db") || connectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlite(connectionString);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

// 2. Add ASP.NET Core Identity with ApplicationUser & simplified password options (allowing "123")
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 1;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
.AddDefaultTokenProviders();

// 3. Configure Cookie Settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

// Configure Forwarded Headers for reverse proxies (Render, Railway, etc.)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// 4. Add MVC Controllers and Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 5. Seed Database, Roles, and Named Accounts on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbInitializer.SeedAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Forwarded headers must run before HttpsRedirection
app.UseForwardedHeaders();

// 6. HTTP Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Assets}/{action=Index}/{id?}");

app.Run();
