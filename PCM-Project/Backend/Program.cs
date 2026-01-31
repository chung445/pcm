using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PCM.API.Data;
using PCM.API.Models;
using PCM.API.Services;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Configure logging (debug) for diagnostics
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Global exception handlers (log unexpected crashes)
AppDomain.CurrentDomain.UnhandledException += (s, e) =>
{
    Console.WriteLine("UNHANDLED EXCEPTION: " + (e.ExceptionObject as Exception)?.ToString());
};
TaskScheduler.UnobservedTaskException += (s, e) =>
{
    Console.WriteLine("UNOBSERVED TASK EXCEPTION: " + e.Exception?.ToString());
    e.SetObserved();
};

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PCM API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyForPCMProject2024MinLength32Characters";

// Validate secret key length early and fail-fast if insufficient (HS256 requires >= 128 bits = 16 bytes)
{
    var keyBytes = Encoding.UTF8.GetBytes(secretKey ?? string.Empty);
    if (keyBytes.Length < 16)
    {
        var msg = $"Invalid JwtSettings: SecretKey must be at least 128 bits (16 bytes). Current length: {keyBytes.Length} bytes. Set JwtSettings__SecretKey environment variable with a secure value.";
        throw new InvalidOperationException(msg);
    }
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IChallengeService, ChallengeService>();
builder.Services.AddScoped<IMatchService, MatchService>();

// CORS Configuration - read ALLOWED_ORIGINS as semicolon-separated list and require it in prod
var allowedOrigins = builder.Configuration["ALLOWED_ORIGINS"]
    ?.Split(';', StringSplitOptions.RemoveEmptyEntries)
    ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("RenderCors", policy =>
    {
        // Require explicit origin(s). If empty, no origins will be allowed (safe default).
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Data Protection - persist keys to filesystem for production (configurable via DataProtection__KeysPath)
var dataProtectionKeysPath = builder.Configuration["DataProtection__KeysPath"] ?? "/srv/data/keys";
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath))
    .SetApplicationName("PCM");

// Cấu hình Forwarded Headers cho Render (Linux/Proxy)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Log CORS origin configuration at startup
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var envAllowed = builder.Configuration["ALLOWED_ORIGINS"];
    if (!string.IsNullOrWhiteSpace(envAllowed))
    {
        logger.LogInformation("CORS allowed origins from ALLOWED_ORIGINS env var: {Origins}", envAllowed);
    }
    else
    {
        logger.LogInformation("No ALLOWED_ORIGINS env var set; using AllowAnyOrigin fallback.");
    }
}

// Ensure DataProtection keys directory exists and log status
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var dpPath = dataProtectionKeysPath;
    try
    {
        if (!Directory.Exists(dpPath)) Directory.CreateDirectory(dpPath);
        logger.LogInformation("DataProtection keys directory ensured: {Path}", dpPath);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Could not create DataProtection keys directory: {Path}", dpPath);
    }
}

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        DbInitializer.Initialize(context, userManager, roleManager).GetAwaiter().GetResult();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Global non-HTTP exception handlers
AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    var ex = e.ExceptionObject as Exception;
    Console.WriteLine("UNHANDLED EXCEPTION: " + ex?.ToString());
};
TaskScheduler.UnobservedTaskException += (sender, e) =>
{
    Console.WriteLine("UNOBSERVED TASK EXCEPTION: " + e.Exception.ToString());
    e.SetObserved();
};

// Sử dụng Forwarded Headers để nhận diện đúng protocol (HTTP/HTTPS) từ Render
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Global exception handler middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (feature?.Error != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(feature.Error, "Global exception caught by middleware");
            Console.WriteLine("Global exception: " + feature.Error.ToString());
        }
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An internal server error occurred.");
    });
});

// Ensure routing is enabled so CORS middleware can handle preflight requests
app.UseRouting();

// Generic CORS request logging to help diagnose CORS issues on Render
app.Use(async (context, next) =>
{
    Console.WriteLine($"[CORS] {context.Request.Method} {context.Request.Path} Origin: {context.Request.Headers["Origin"]}");
    await next();
});

// Use the RenderCors policy (must be registered earlier)
app.UseCors("RenderCors");


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Thêm endpoint gốc để kiểm tra server sống hay chết
app.MapGet("/", () => "PCM API is running! Go to /swagger for API documentation.");

var lifetime = app.Lifetime;
lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("=== APPLICATION STARTED ===");
});
lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("=== APPLICATION STOPPING ===");
});
lifetime.ApplicationStopped.Register(() =>
{
    Console.WriteLine("=== APPLICATION STOPPED ===");
});

AppDomain.CurrentDomain.ProcessExit += (s, e) =>
{
    try
    {
        var info = $"ProcessExit at {DateTime.UtcNow:o}\n";
        info += $"Threads: {System.Diagnostics.Process.GetCurrentProcess().Threads.Count}\n";
        var path = System.IO.Path.Combine(Environment.CurrentDirectory, "shutdown_debug.log");
        System.IO.File.WriteAllText(path, info);
        Console.WriteLine("Wrote shutdown_debug.log: " + path);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error writing shutdown log: " + ex);
    }
};

app.Run();
