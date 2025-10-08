using CartAPI.Config;
using CartAPI.Data;
using CartAPI.Services;
using CartAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

AddLogging(builder);
ConfigureOpenTelemetry(builder);

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));

ConfigureDatabase(builder);
InjectServices(builder);
ConfigureJWT(builder);

builder.Services.AddEndpointsApiExplorer();

ConfigureSwagger(builder);

builder.Services.AddControllers();

builder.Services.AddAuthorization();

var app = builder.Build();

await RunMigrationsIfNeeded(args, app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static void AddLogging(WebApplicationBuilder builder)
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();
    builder.Logging.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("CartAPI"));
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
    });
}

static void ConfigureOpenTelemetry(WebApplicationBuilder builder)
{
    var serviceName = "CartAPI";
    var serviceVersion = "1.0.0";

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.EnrichWithHttpRequest = (activity, httpRequest) =>
                {
                    activity.SetTag("http.request.path", httpRequest.Path);
                    activity.SetTag("http.request.method", httpRequest.Method);
                };
                options.EnrichWithHttpResponse = (activity, httpResponse) =>
                {
                    activity.SetTag("http.response.status_code", httpResponse.StatusCode);
                };
            })
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            )
        .WithMetrics(metrics => metrics
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            );
}

static void ConfigureDatabase(WebApplicationBuilder builder)
{
    var connectionString =
        builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string"
            + "'DefaultConnection' not found.");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));
}

static void ConfigureJWT(WebApplicationBuilder builder)
{
    var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfig>()!;

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtConfig.Key)
                )
            };
        });
}

static void ConfigureSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Deck Cart API",
            Version = "v1",
            Description = "An API for managing users, items, and carts."
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer' followed by a space and your token.\n\nExample: **Bearer eyJhbGciOi...**"
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
            Array.Empty<string>()
        }
    });
    });
}

static async Task RunMigrationsIfNeeded(string[] args, WebApplication app)
{
    var argsList = args.ToList();

    if (argsList.Contains("--migrate"))
    {
        using (var scope = app.Services.CreateScope())
        {
            using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!db.Database.GetService<IRelationalDatabaseCreator>().Exists())
                await db.Database.MigrateAsync();
        }

        // Remove the flag so the app runs normally after migration
        argsList.Remove("--migrate");
    }
}

static void InjectServices(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IJwtServices, JwtServices>();
    builder.Services.AddScoped<ICartService, CartService>();
    builder.Services.AddScoped<IUserService, UserService>();
}