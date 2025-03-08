using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FullWebApi.Application.Interfaces;
using FullWebApi.Application.Services;
using FullWebApi.Domain.Dtos;
using FullWebApi.Infrastructure.Data;

namespace FullWebApi.Api.EndPoints;

public class GetUserById : Endpoint<UserDto>
{
  private readonly IUserServices _userServices;

  public GetUserById(IUserServices userServices)
  {
    _userServices = userServices;
  }


  public override void Configure()
  {
    Get("api/user/{id}");
    Roles("Admin");
  }

  public override async Task HandleAsync(UserDto req ,CancellationToken ct)
  {
    var id = Route<int>("id");
    
    try
    {
      var user = await _userServices.GetUser(id);
      await SendOkAsync(user, ct);  
      return;
    }
    catch (System.Exception)
    {
      await SendAsync("The required user doesn't exist", 404, cancellation: ct);
      return;
    } 
  }
}
