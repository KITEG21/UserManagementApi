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

public class GetUserById : EndpointWithoutRequest<UserDto>
{
  private readonly IUserServices _userServices;
  private readonly UserMapper _mapper;

  public GetUserById(IUserServices userServices, UserMapper mapper)
  {
    _userServices = userServices;
    _mapper = mapper;
  }

  public override void Configure()
  {
    Get("/api/user/{id}");
    Roles("Admin");
    Tags("Users");
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var id = Route<Guid>("id");
    
    var user = await _userServices.GetUser(id);
    
    if (user == null)
    {
      Log.Information("User not found with id: {id}", id);
      ThrowError("User not found", 404);
    }
    
    var userDto = _mapper.UserToUserDto(user);
    await Send.OkAsync(userDto, ct);
  }
}
