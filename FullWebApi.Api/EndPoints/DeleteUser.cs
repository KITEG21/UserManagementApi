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

namespace FullWebApi.Api.EndPoints;

public class DeleteUser : Endpoint<DeleteUserRequest>
{
    private readonly AppDBContext _context;
    private readonly IUserServices _userServices;

    public DeleteUser(AppDBContext context, IUserServices userServices)
    {
      _context = context;
      _userServices = userServices;
    }

    public override void Configure()
    {
      Delete("api/user/deleteuser/{id}");
      Roles("Admin");
    }

    public override async Task HandleAsync(DeleteUserRequest req, CancellationToken ct)
    {
      var id = Route<int>("id");
      var success = await _userServices.DeleteUser(id);

      if (!success)
      {
        var errorResponse = new
        {
          StatusCode = 404,
          Error = "The required user doesn't exist"
        };

        await SendAsync(errorResponse, 404, ct);
        Log.Information("Delete user failed attempt: The required user Id couldn't be found. Id: {id}", id);
        return;
      }

        await SendAsync("The user was deleted successfully", cancellation: ct);
        Log.Information("User deleted successfully. Id: {id}", id);
    }
}
