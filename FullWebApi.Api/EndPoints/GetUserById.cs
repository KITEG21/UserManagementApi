using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FullWebApi.Application.Interfaces;
using FullWebApi.Domain.Dtos;
using FullWebApi.Infrastructure.Data;

namespace FullWebApi.Api.EndPoints;

public class GetUserById : Endpoint<UserDto>
{
  private readonly AppDBContext _context;
  private readonly IUserServices _userServices;

  public GetUserById(AppDBContext context, IUserServices userServices)
  {
    _context = context;
    _userServices = userServices;
  }

  public override void Configure()
  {
    Get("api/user/{id}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(UserDto req ,CancellationToken ct)
  {
    var id = Route<int>("id");
    var user = await _userServices.GetUser(id);
    if(user == null)
      await SendAsync("The required user doesn't exist", 404, cancellation: ct);

    await SendOkAsync(user, ct);  
  }
}
