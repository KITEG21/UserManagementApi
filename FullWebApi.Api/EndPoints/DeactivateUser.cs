using FastEndpoints;
using FullWebApi.Application.Interfaces;
using Serilog;

namespace FullWebApi.Api.EndPoints;

public class DeactivateUserRequest
{
  public string? Reason { get; set; }
}

public class DeactivateUser : Endpoint<DeactivateUserRequest>
{
  private readonly IUserServices _userServices;

  public DeactivateUser(IUserServices userServices)
  {
    _userServices = userServices;
  }

  public override void Configure()
  {
    Post("/api/user/{id}/deactivate");
    Roles("Admin");
    Tags("Users");
    Summary(s =>
    {
      s.Summary = "Deactivate a user account";
      s.Description = "Sets the user status to Inactive with an optional reason";
    });
  }

  public override async Task HandleAsync(DeactivateUserRequest req, CancellationToken ct)
  {
    var userId = Route<Guid>("id");
    
    var success = await _userServices.DeactivateUser(userId, req.Reason);
    
    if (!success)
    {
      Log.Information("Deactivate user failed: User not found. Id: {id}", userId);
      ThrowError("User not found", 404);
    }
    
    Log.Information("User deactivated successfully. Id: {id}, Reason: {reason}", userId, req.Reason);
    await Send.OkAsync(new { Message = "User deactivated successfully" }, ct);
  }
}
