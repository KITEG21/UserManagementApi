using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FullWebApi.Application.Interfaces;
using Serilog;

namespace FullWebApi.Api.EndPoints;

public class DeleteUser : EndpointWithoutRequest
{
  private readonly IUserServices _userServices;
  
  public DeleteUser(IUserServices userServices)
  {
    _userServices = userServices;
  }

  public override void Configure()
  {
    Delete("/api/user/deleteuser/{id}");
    Roles("Admin");
    Tags("Users");
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var id = Route<Guid>("id");
    var success = await _userServices.DeleteUser(id);

    if (!success)
    {
      Log.Information("Delete user failed: User not found. Id: {id}", id);
      ThrowError("User not found", 404);
    }

    Log.Information("User deleted successfully. Id: {id}", id);
    await Send.OkAsync(new { Message = "User deleted successfully" }, ct);
  }
}
