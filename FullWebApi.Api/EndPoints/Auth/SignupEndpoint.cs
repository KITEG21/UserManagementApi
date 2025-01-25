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

public class SignUpEndpoint : Endpoint<SignUpRequest>
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
  }

  public override async Task HandleAsync(SignUpRequest req, CancellationToken ct)
  {
    SignUpRequestValidator validator = new();
    ValidationResult result = validator.Validate(req);

    // Check if the admin user already exists
    if (await _context.AdminUsers.AnyAsync(u => u.Username == req.Username || u.Email == req.Email, ct))
    {
      var errorResponse = new
      {
        StatusCode = 409,
        Errors = "User already exist"
      };  
      await SendAsync(errorResponse, 409, ct);
      Log.Information("SignUp attempt failed: User already exist. Username: {Username}, Email: {Email}", req.Username, req.Email);
      return;
    }

    if(!result.IsValid)
    {
      var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
      var errorResponse = new
      {
        StatusCode = 400,
        Errors = errorMessages
      };

      await SendAsync(errorResponse, 400, ct);
      Log.Information("SignUp attempt failed: Validation errors. Username: {Username}, Email: {Email}, Errors: {Errors}", req.Username, req.Email, errorMessages);
      return;
    }

    // Create a new admin user
    var adminUser = new AdminUser
    {
      Username = req.Username,
      Email = req.Email,
    };
    //Hashes the password
    adminUser.Password = _passwordHasher.HashPassword(adminUser, req.Password);

    _context.AdminUsers.Add(adminUser);
    await _context.SaveChangesAsync(ct);

    // Generate JWT token
    var token = _tokenService.GenerateToken(adminUser.Username, "Admin");
    await SendAsync(new SignUpResponse { Token = token }, cancellation: ct);

    Log.Information("New admin user signed up successfully");
 }
}
