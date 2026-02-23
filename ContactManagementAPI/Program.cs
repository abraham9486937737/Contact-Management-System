using Microsoft.EntityFrameworkCore;
using ContactManagementAPI.Data;
using ContactManagementAPI.Services;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Prevent circular references
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(2); // Reduced from 8 to 2 hours to prevent memory buildup
});

// Add memory cache with size limit
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024; // Limit cache size
});

// Configure Antiforgery with SameSite settings
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Configure database connection (SQLite preferred for portable installs)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var useSqlite = connectionString.TrimStart().StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase);

if (useSqlite)
{
    var sqliteBuilder = new SqliteConnectionStringBuilder(connectionString);
    var sqliteDbPathOverride = Environment.GetEnvironmentVariable("SQLITE_DB_PATH");
    var dataSource = sqliteBuilder.DataSource;

    if (!string.IsNullOrWhiteSpace(sqliteDbPathOverride))
    {
        dataSource = sqliteDbPathOverride;
    }

    if (string.IsNullOrWhiteSpace(dataSource))
    {
        dataSource = "ContactManagement.db";
    }

    if (!Path.IsPathRooted(dataSource))
    {
        var appDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ContactManagementSystem");

        Directory.CreateDirectory(appDataFolder);
        dataSource = Path.Combine(appDataFolder, dataSource);
    }

    sqliteBuilder.DataSource = dataSource;
    connectionString = sqliteBuilder.ToString();
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useSqlite)
    {
        options.UseSqlite(connectionString);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }

    // Optimize EF Core for better memory management
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    options.EnableSensitiveDataLogging(false);
    options.EnableDetailedErrors(false);
});

// Add custom services
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddScoped<UserContextService>();
builder.Services.AddScoped<ImportExportService>();
builder.Services.AddScoped<ContactStatisticsService>();

// Configure CORS if needed for future API consumption
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5000", "https://localhost:5001")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Support reverse proxies/load balancers in cloud hosting
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

var disableHttpsRedirection =
    app.Configuration.GetValue<bool>("DisableHttpsRedirection") ||
    string.Equals(Environment.GetEnvironmentVariable("DISABLE_HTTPS_REDIRECTION"), "true", StringComparison.OrdinalIgnoreCase);

// Ensure database exists and seed defaults
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (useSqlite)
    {
        dbContext.Database.EnsureCreated();
    }
    else
    {
        dbContext.Database.Migrate();
    }

    SeedData.Initialize(dbContext);
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!disableHttpsRedirection)
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
