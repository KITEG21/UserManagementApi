using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FullWebApi.Application.Interfaces;
using FullWebApi.Domain.Models.Auth;
using FullWebApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FullWebApi.Api.EndPoints.Auth
{
    public class LoginEndpoint : Endpoint<LoginRequest>
    {
        private readonly ITokenServices _tokenServices;
        private readonly AppDBContext _context;
        public LoginEndpoint(ITokenServices tokenServices, AppDBContext context)
        {
            _tokenServices = tokenServices;
            _context = context;
        }

        public override void Configure()
        {
            Post("/api/auth/login");
            AllowAnonymous();
        }
        public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
        {
            
            var user = await _context.AdminUsers.FirstOrDefaultAsync(x => x.Username == req.Username && x.Password == req.Password);

            if(user == null)
            {
                var errorResponse = new 
                {
                    StatusCode = 401,
                    Error = "Invalid credentials"
                };


                await SendAsync(errorResponse, 401, ct);
                Log.Information("Login attempt failed: Invalid credentials. Username: {Username}, Password: {Password}", req.Username, req.Password);
                return;
            }

            var token = _tokenServices.GenerateToken(req.Username, "Admin");
            await SendAsync(new LoginResponse {Token = token}, cancellation: ct);
            Log.Information("User logged successfully. Username: {Username}", user.Username);
        }
    }
}