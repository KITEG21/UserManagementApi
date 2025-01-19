using FullWebApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FastEndpoints;
using FluentValidation;
using FullWebApi.Domain.ModelsValidator;
using FullWebApi.Application.Interfaces;
using FullWebApi.Application.Services;
using FullWebApi.Application.Mappings;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<UserMapper>();

builder.Services.AddDbContext<AppDBContext>(options 
    => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("FullWebApi.Api")));

var app = builder.Build();

app.UseFastEndpoints();

app.Run();

