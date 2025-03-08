using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FullWebApi.Application.Interfaces;
using FullWebApi.Application.Services;
using FullWebApi.Domain.Dtos;
using FullWebApi.Infrastructure.Data;
using FullWebApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FullWebApi.Api.EndPoints;

public class GetAllUsers : EndpointWithoutRequest<List<UserDto>>
{

  private readonly IUserServices _userServices;
  public GetAllUsers(IUserServices userServices)
  {
    _userServices = userServices;
  }

  public override void Configure()
  {
    Get("/api/user/users");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var users = await _userServices.GetAllUsers();
    if (users == null || !users.Any())
    {
      await SendNotFoundAsync(cancellation: ct);
      return;
    }
    
    await SendOkAsync(users, ct);
  }

}
