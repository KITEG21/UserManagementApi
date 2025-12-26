using FullWebApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation;
using FullWebApi.Domain.ModelsValidator;
using FullWebApi.Application.Interfaces;
using FullWebApi.Application.Services;
using FullWebApi.Application.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using FullWebApi.Domain.Models.Auth;
using Serilog;
using Microsoft.OpenApi.Models;
using FullWebApi.Infrastructure.Repositories;
using FullWebApi.Infrastructure.DependencyInjection;
using FullWebApi.Infrastructure.UnitOfWork;
using FullWebApi.Infrastructure.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Configure Logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
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
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new ArgumentNullException("JwtSettings:Key")))
    };
});

// Adding services to the container
builder.Services.AddFastEndpoints();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
builder.Services.AddAuthorization();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<UserMapper>();
builder.Services.AddScoped<ITokenServices, TokenServices>();
builder.Services.AddScoped<IPasswordHasher<LoginRequest>, PasswordHasher<LoginRequest>>();
builder.Services.AddScoped<IPasswordHasher<AdminUser>, PasswordHasher<AdminUser>>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<SeederRunner>();
builder.Services.AddScoped<ISeeder, UserSeeder>();
builder.Services.AddMemoryCache();
builder.Services.AddInfrastructure();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {
        Title = "UserManagement API",
        Version = "v1",
        Description = "A secure RESTful API for user management with JWT authentication"
    });

    // Group endpoints by route path: /api/auth/* => Authentication, /api/user/* => Users
    c.TagActionsBy(api =>
    {
        var path = api.RelativePath?.ToLowerInvariant() ?? "";
        if (path.Contains("/api/auth/") || path.StartsWith("api/auth")) return new[] { "Authentication" };
        if (path.Contains("/api/user/") || path.StartsWith("api/user")) return new[] { "Users" };
        return new[] { "Api" }; // fallback
    });

    c.DocInclusionPredicate((name, api) => true);

    // JWT Bearer config for Swagger UI
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        BearerFormat = "JWT",
        Scheme = "bearer",
        Type = SecuritySchemeType.Http,
        In = ParameterLocation.Header,
        Description = "Enter JWT token",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, Array.Empty<string>() } });
});


// Configure Database Context
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("FullWebApi.Api")));

// Add Swagger with FastEndpoints
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "UserManagement API";
        s.Version = "v1";
        s.Description = "A secure RESTful API for user management with JWT authentication";
    };
    
    o.EnableJWTBearerAuth = true;
});

var app = builder.Build();

// Adding seeders services - temporarily disabled due to Neon DB sleep
// Uncomment after waking up the database in Neon console
// using(var scope = app.Services.CreateScope())
// {
//     var seederRunner = scope.ServiceProvider.GetRequiredService<SeederRunner>();
//     await seederRunner.RunAsync(); 
// }

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserManagement API v1");
    c.RoutePrefix = string.Empty; // serve swagger at site root
});

app.UseHttpsRedirection();
app.UseFastEndpoints();
app.UseAuthentication();
app.UseAuthorization();

// Use FastEndpoints Swagger
app.UseSwaggerGen();

app.Run();