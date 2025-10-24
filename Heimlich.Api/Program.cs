using AutoMapper;
using Heimlich.Application.Features.Auth.Handlers;
using Heimlich.Application.Mapping;
using Heimlich.Domain.Entities;
using Heimlich.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Application Insights: register telemetry and enable logging to AI
builder.Services.AddApplicationInsightsTelemetry();
builder.Logging.AddApplicationInsights();

var config = builder.Configuration;
var connectionString = config.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Cadena de conexión no encontrada.");
builder.Services.AddDbContext<HeimlichDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<HeimlichDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegisterHandler>());

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // Use camelCase naming for JSON by default so Swagger reflects client JSON names
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        // Permitir enums como strings en JSON (ForceSensor, TouchSensor, etc.)
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();

// Read flags from configuration so behavior in production can be controlled from App Service settings
var enableSwagger = builder.Configuration.GetValue<bool>("EnableSwagger", false);
var applyMigrationsOnStartup = builder.Configuration.GetValue<bool>("ApplyMigrationsOnStartup", false);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Heimlich API", Version = "v1" });

    // Enable XML comments if available
    var xmlFile = System.IO.Path.ChangeExtension(System.Reflection.Assembly.GetEntryAssembly().Location, ".xml");
    if (System.IO.File.Exists(xmlFile)) c.IncludeXmlComments(xmlFile);

    // Configuración para JWT Bearer
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddSingleton(provider =>
{
    var configMapper = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile(new AutoMapperProfile());
    });
    return configMapper.CreateMapper();
});

// CORS seguro para producción
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMobile", policy =>
        policy.WithOrigins(
            "https://TU-APP-MOBILE.azurestaticapps.net", // Cambia por tu dominio real
            "https://localhost:4200" // Permite localhost solo para pruebas locales
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

var app = builder.Build();

// Ejecutar migraciones y seeding solo si está explicitamente habilitado en configuración
if (applyMigrationsOnStartup)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<HeimlichDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        db.Database.Migrate();
        Heimlich.Infrastructure.Identity.SeedData.InitializeAsync(userManager, roleManager, db).GetAwaiter().GetResult();
    }
}

// Swagger enabled only in Development OR if explicitly enabled by configuration (disabled by default in production)
if (app.Environment.IsDevelopment() || enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Heimlich API v1");
        c.DefaultModelsExpandDepth(-1); // hide schemas until expanded
    });
}

// Production-specific middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("An unexpected server error occurred.");
        });
    });

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowMobile");
app.MapControllers();
app.Run();