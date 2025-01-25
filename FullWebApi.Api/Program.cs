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

var builder = WebApplication.CreateBuilder(args);

//Configure Logger
Log.Logger = new LoggerConfiguration()
.WriteTo.Console()
.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();

builder.Host.UseSerilog();

var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = 
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


builder.Services.AddFastEndpoints();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<UserMapper>();
builder.Services.AddScoped<ITokenServices, TokenServices>();
builder.Services.AddScoped<IPasswordHasher<AdminUser>, PasswordHasher<AdminUser>>();

builder.Services.AddDbContext<AppDBContext>(options 
    => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("FullWebApi.Api")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseFastEndpoints();
app.UseAuthentication();
app.UseAuthorization();

app.Run();

