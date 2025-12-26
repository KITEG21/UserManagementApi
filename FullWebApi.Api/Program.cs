using FullWebApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FastEndpoints;
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

// Add Swagger (enabled for all environments)
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "UserManagement API", 
        Version = "v1",
        Description = "A secure RESTful API for user management with JWT authentication",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com"
        }
    });

    // Add JWT Authentication to Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
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

// Configure Database Context
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("FullWebApi.Api")));

var app = builder.Build();

// Adding seeders services
using(var scope = app.Services.CreateScope())
{
    var seederRunner = scope.ServiceProvider.GetRequiredService<SeederRunner>();
    await seederRunner.RunAsync(); 
}

// Configure the HTTP request pipeline
// Enable Swagger in all environments for Azure
app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserManagement API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the root (https://yourapp.azurewebsites.net/)
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseFastEndpoints();
app.UseAuthentication();
app.UseAuthorization();

app.Run();