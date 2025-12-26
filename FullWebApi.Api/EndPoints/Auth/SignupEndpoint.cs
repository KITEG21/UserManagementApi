using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FullWebApi.Infrastructure.Data;
using FullWebApi.Domain.Models.Auth;
using FullWebApi.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using FullWebApi.Domain.ModelsValidator;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace FullWebApi.Api.EndPoints.Auth;

public class SignUpEndpoint : Endpoint<SignUpRequest, SignUpResponse>
{
  private readonly AppDBContext _context;
  private readonly ITokenServices _tokenService;
  private readonly IPasswordHasher<AdminUser> _passwordHasher;

  public SignUpEndpoint(AppDBContext context, ITokenServices tokenService, IPasswordHasher<AdminUser> passwordHasher)
  {
    _context = context;
    _tokenService = tokenService;
    _passwordHasher = passwordHasher;
  }

  public override void Configure()
  {
    Post("/api/auth/signup");
    AllowAnonymous();
    Tags("Authentication");
  }

  public override async Task HandleAsync(SignUpRequest req, CancellationToken ct)
  {
    SignUpRequestValidator validator = new();
    ValidationResult result = validator.Validate(req);

    // Check if the admin user already exists
    if (await _context.AdminUsers.AnyAsync(u => u.Username == req.Username || u.Email == req.Email, ct))
    {
      Log.Information("SignUp attempt failed: User already exists. Username: {Username}, Email: {Email}", req.Username, req.Email);
      ThrowError("User already exists", 409);
    }

    if (!result.IsValid)
    {
      var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
      Log.Information("SignUp attempt failed: Validation errors. Username: {Username}, Email: {Email}", req.Username, req.Email);
      foreach (var error in result.Errors)
      {
        AddError(error.PropertyName, error.ErrorMessage);
      }
      ThrowIfAnyErrors();
    }

    // Create a new admin user
    var adminUser = new AdminUser
    {
      Username = req.Username,
      Email = req.Email,
      Password = _passwordHasher.HashPassword(new AdminUser(), req.Password)
    };

    _context.AdminUsers.Add(adminUser);
    await _context.SaveChangesAsync(ct);

    // Generate JWT token
    var token = _tokenService.GenerateToken(adminUser.Username, "Admin");
    Log.Information("New admin user signed up successfully. Username: {Username}", adminUser.Username);
    await Send.CreatedAtAsync<LoginEndpoint>(null, new SignUpResponse { Token = token }, cancellation: ct);
  }
}
