using FastEndpoints;
using FullWebApi.Application.Interfaces;
using FullWebApi.Domain.Dtos;
using Serilog;

namespace FullWebApi.Api.EndPoints;

public class SuspendUserRequest
{
  public string? Reason { get; set; }
}

public class SuspendUser : Endpoint<SuspendUserRequest>
{
  private readonly IUserServices _userServices;

  public SuspendUser(IUserServices userServices)
  {
    _userServices = userServices;
  }

  public override void Configure()
  {
    Post("/api/user/{id}/suspend");
    Roles("Admin");
    Tags("Users");
    Summary(s =>
    {
      s.Summary = "Suspend a user account";
      s.Description = "Sets the user status to Suspended with an optional reason";
    });
  }

  public override async Task HandleAsync(SuspendUserRequest req, CancellationToken ct)
  {
    var userId = Route<Guid>("id");
    
    var success = await _userServices.SuspendUser(userId, req.Reason);
    
    if (!success)
    {
      Log.Information("Suspend user failed: User not found. Id: {id}", userId);
      ThrowError("User not found", 404);
    }
    
    Log.Information("User suspended successfully. Id: {id}, Reason: {reason}", userId, req.Reason);
    await Send.OkAsync(new { Message = "User suspended successfully" }, ct);
  }
}
