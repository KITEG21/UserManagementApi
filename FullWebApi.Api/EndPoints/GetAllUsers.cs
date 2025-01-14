using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FullWebApi.Application.Interfaces;
using FullWebApi.Domain.Dtos;
using FullWebApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FullWebApi.Api.EndPoints
{
    public class GetAllUsers : Endpoint<UserDto>
    {

        private readonly AppDBContext _context;
        private readonly IUserServices _userServices;
        public GetAllUsers(AppDBContext context, IUserServices userServices)
        {
            _context = context;
            _userServices = userServices;
        }

        public override void Configure()
        {
            Get("/api/user/users");
            AllowAnonymous();
        }

        public override async Task HandleAsync(UserDto req, CancellationToken ct)
        {
            var users = await _userServices.GetAllUsers(req);
            await SendOkAsync(users, cancellation: ct);
        }
    }
}