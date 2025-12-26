using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FullWebApi.Application.Interfaces;
using FullWebApi.Domain.Models.Auth;
using FullWebApi.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FullWebApi.Api.EndPoints.Auth;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly ITokenServices _tokenServices;
    private readonly AppDBContext _context;
    private readonly IPasswordHasher<LoginRequest> _passwordHasher;

    public LoginEndpoint(ITokenServices tokenServices, AppDBContext context, IPasswordHasher<LoginRequest> passwordHasher)
    {
        _tokenServices = tokenServices;
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
        Tags("Authentication");
    }

    private bool CheckPassword(LoginRequest req, string hashedPassword, string passwordToCheck)
    {
        var result = _passwordHasher.VerifyHashedPassword(req, hashedPassword, passwordToCheck);
        return result != PasswordVerificationResult.Failed;
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _context.AdminUsers.FirstOrDefaultAsync(x => x.Username == req.Username, ct);

        if (user == null)
        {
            Log.Information("Login attempt failed: User not found. Username: {Username}", req.Username);
            ThrowError("Invalid credentials", 401);
        }

        if (!CheckPassword(req, user!.Password, req.Password))
        {
            Log.Information("Login attempt failed: Invalid password. Username: {Username}", req.Username);
            ThrowError("Invalid credentials", 401);
        }

        var token = _tokenServices.GenerateToken(req.Username, "Admin");
        Log.Information("User logged in successfully. Username: {Username}", user.Username);
        await Send.OkAsync(new LoginResponse { Token = token }, ct);
    }
}