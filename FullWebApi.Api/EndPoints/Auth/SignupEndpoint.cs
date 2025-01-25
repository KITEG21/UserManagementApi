using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FullWebApi.Infrastructure.Data;
using FullWebApi.Domain.Models.Auth;
using FullWebApi.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FullWebApi.Api.EndPoints.Auth
{
    public class SignUpEndpoint : Endpoint<SignUpRequest, SignUpResponse>
    {
        private readonly AppDBContext _context;
        private readonly ITokenServices _tokenService;

        public SignUpEndpoint(AppDBContext context, ITokenServices tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public override void Configure()
        {
            Post("/api/auth/signup");
            AllowAnonymous();
        }

        public override async Task HandleAsync(SignUpRequest req, CancellationToken ct)
        {
            // Check if the admin user already exists
            if (await _context.AdminUsers.AnyAsync(u => u.Username == req.Username || u.Email == req.Email, ct))
            {
                await SendErrorsAsync();
                return;
            }

            // Create a new admin user
            var adminUser = new AdminUser
            {
                Username = req.Username,
                Email = req.Email,
                Password = req.Password // Note: In a real application, you should hash the password before saving it
            };

            _context.AdminUsers.Add(adminUser);
            await _context.SaveChangesAsync(ct);

            // Generate JWT token
            var token = _tokenService.GenerateToken(adminUser.Username, "Admin");
            await SendAsync(new SignUpResponse { Token = token }, cancellation: ct);
        }
    }
}