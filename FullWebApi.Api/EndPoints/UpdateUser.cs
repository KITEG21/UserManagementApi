using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FullWebApi.Application.Interfaces;
using FullWebApi.Application.Mappings;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Models;
using Serilog;

namespace FullWebApi.Api.EndPoints;

public class UpdateUser : Endpoint<User>
{
  private readonly IUserServices _userServices;
  private readonly UserMapper _mapper;

  public UpdateUser(IUserServices userServices, UserMapper mapper)
  {
    _userServices = userServices;
    _mapper = mapper;
  }

  public override void Configure()
  {
    Put("/api/user/{id}");
    Roles("Admin");
    Tags("Users");
  }

  public override async Task HandleAsync(User req, CancellationToken ct)
  {
    var id = Route<Guid>("id");
    req.Id = id; // Ensure we update the correct user from route

    var userUpdate = await _userServices.UpdateUser(req);
    
    if (userUpdate == null)
    {
      Log.Information("Update user failed: User not found. Id: {id}", id);
      ThrowError("User not found", 404);
    }
    
    Log.Information("User updated successfully. Id: {id}", id);
    var userDto = _mapper.UserToUserDto(userUpdate);
    await Send.OkAsync(userDto, ct);
  }
}
