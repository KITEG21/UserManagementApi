using FastEndpoints;
using FullWebApi.Application.Interfaces;
using Serilog;

namespace FullWebApi.Api.EndPoints;

public class ActivateUser : EndpointWithoutRequest
{
  private readonly IUserServices _userServices;

  public ActivateUser(IUserServices userServices)
  {
    _userServices = userServices;
  }

  public override void Configure()
  {
    Post("/api/user/{id}/activate");
    Roles("Admin");
    Tags("Users");
    Summary(s =>
    {
      s.Summary = "Activate a user account";
      s.Description = "Sets the user status to Active";
    });
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var userId = Route<Guid>("id");
    
    var success = await _userServices.ActivateUser(userId);
    
    if (!success)
    {
      Log.Information("Activate user failed: User not found. Id: {id}", userId);
      ThrowError("User not found", 404);
    }
    
    Log.Information("User activated successfully. Id: {id}", userId);
    await Send.OkAsync(new { Message = "User activated successfully" }, ct);
  }
}
