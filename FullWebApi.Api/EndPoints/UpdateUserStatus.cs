using FastEndpoints;
using FullWebApi.Application.Interfaces;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Enums;
using Serilog;

namespace FullWebApi.Api.EndPoints;

public class UpdateUserStatus : Endpoint<UpdateUserStatusRequest>
{
  private readonly IUserServices _userServices;

  public UpdateUserStatus(IUserServices userServices)
  {
    _userServices = userServices;
  }

  public override void Configure()
  {
    Patch("/api/user/{id}/status");
    Roles("Admin");
    Tags("Users");
    Summary(s =>
    {
      s.Summary = "Update user status";
      s.Description = "Update the status of a user account (PendingVerification=0, Active=1, Inactive=2, Suspended=3)";
    });
  }

  public override async Task HandleAsync(UpdateUserStatusRequest req, CancellationToken ct)
  {
    var userId = Route<Guid>("id");
    
    var success = await _userServices.UpdateUserStatus(userId, req.Status, req.Reason);
    
    if (!success)
    {
      Log.Information("Update user status failed: User not found. Id: {id}", userId);
      ThrowError("User not found", 404);
    }
    
    Log.Information("User status updated successfully. Id: {id}, Status: {status}", userId, req.Status);
    await Send.OkAsync(new { Message = $"User status updated to {req.Status}" }, ct);
  }
}
