using AccountingVAS.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using ApplicationUser = global::Web.Models.Identity.ApplicationUser;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    var seqUrl = context.Configuration["Serilog:Seq:ServerUrl"];
    var minimumLevel = context.HostingEnvironment.IsDevelopment() ? LogEventLevel.Debug : LogEventLevel.Information;

    configuration
        .MinimumLevel.Is(minimumLevel)
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: "logs/log-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            shared: true);

    if (!string.IsNullOrWhiteSpace(seqUrl))
    {
        configuration.WriteTo.Seq(seqUrl);
    }
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AccountingDbContext>()
    .AddDefaultTokenProviders();

// Configure DbContext with triple provider support via Database:Provider.
builder.Services.AddDbContext<AccountingDbContext>((serviceProvider, options) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var environment = serviceProvider.GetRequiredService<IHostEnvironment>();

    var provider = configuration["Database:Provider"] ?? "SQLite";
    var connectionString = configuration.GetConnectionString(provider)
        ?? throw new InvalidOperationException($"Missing connection string for provider '{provider}'.");

    ConfigureDatabaseProvider(options, provider, connectionString);

    if (environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSerilogRequestLogging();

app.UseAuthentication();
app.Use(async (httpContext, next) =>
{
    var userId = httpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    using (LogContext.PushProperty("RequestPath", httpContext.Request.Path.Value ?? string.Empty))
    using (LogContext.PushProperty("Method", httpContext.Request.Method))
    using (LogContext.PushProperty("UserId", userId ?? "Anonymous"))
    {
        await next();
    }
});
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Initialize database: apply migrations and seed VAS core CoA.
try
{
    await using var serviceScope = app.Services.CreateAsyncScope();
    var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var context = serviceScope.ServiceProvider.GetRequiredService<AccountingDbContext>();

    logger.LogInformation("Starting database initialization (migrations and seeding)...");

    await context.Database.MigrateAsync();
    await DatabaseSeeder.SeedVasCoaIfNeededAsync(context);
    await IdentitySeeder.SeedRolesAndAdminAsync(serviceScope.ServiceProvider);

    logger.LogInformation("Database initialization completed successfully.");
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during database initialization. The application will continue, but the database may not be initialized.");
}

app.Run();

static void ConfigureDatabaseProvider(DbContextOptionsBuilder options, string provider, string connectionString)
{
    switch (provider)
    {
        case "SQLite":
            options.UseSqlite(connectionString);
            break;
        case "SqlServerExpress":
        case "SqlServer":
            options.UseSqlServer(connectionString);
            break;
        default:
            throw new InvalidOperationException(
                $"Unsupported database provider '{provider}'. Supported values: SQLite, SqlServerExpress, SqlServer.");
    }
}

public partial class Program;
