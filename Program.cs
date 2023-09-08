using IpDeputyApi.Authentication;
using IpDeputyApi.Database;
using IpDeputyApi.Middleware;
using IpDeputyApi.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure config file
builder.Configuration.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "Settings", "appsettings.json"));

// Logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger();

// Add logger
builder.Host.UseSerilog();

// Configure Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Ip Deputy Bot API",
        Description = "API for telegram bot Ip Deputy Bot",
        Contact = new OpenApiContact
        {
            Name = "Github",
            Url = new Uri("https://github.com/Navatusein/Ip-Deputy-3.0")
        },
        License = new OpenApiLicense
        {
            Name = "License",
            Url = new Uri("https://github.com/Navatusein/Ip-Deputy-3.0/blob/main/LICENSE")
        }
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            jwtSecurityScheme, Array.Empty<string>()
        }
    });
});

// Configure Database
builder.Services.AddDbContextPool<IpDeputyDbContext>(options =>
{
    options.UseLazyLoadingProxies();

    switch (builder.Configuration["Database:Provider"])
    {
        case "Sqlite":
            options.UseSqlite(builder.Configuration["Database:ConnectionString"]);
            break;
        case "PostgreSQL":
            options.UseNpgsql(builder.Configuration["Database:ConnectionString"]);
            break;
        default:
            break;
    }
});

// Configure Frontend Authentication Service
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["FrontendAuthorizeJWT:Issuer"],
            ValidAudience = builder.Configuration["FrontendAuthorizeJWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["FrontendAuthorizeJWT:Key"]))
        };
    });

// Configure Bot Authentication Service
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<BotAuthenticationSchemeOptions, BotAuthenticationHandler>(
        BotAuthenticationSchemeOptions.DefaultSchemeName,
        options =>
        {
            options.BotToken = builder.Configuration["BotAuthorizeToken"];
        }
    );

// Configure Automapper
builder.Services.AddAutoMapper(typeof(AppMappingProfile));

// JwtHelper 
builder.Services.AddSingleton<JwtHelper>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options => {
    string[] origins = builder.Configuration.GetSection("Origins").Get<string[]>()!;

    options.WithOrigins(origins);
    options.AllowAnyMethod();
    options.AllowAnyHeader();
    options.AllowCredentials();
});

app.UseAuthentication();

app.UseAuthorization();

//app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Logger.LogInformation("Version: (#verion)");

app.Run();
