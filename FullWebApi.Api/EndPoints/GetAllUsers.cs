using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FullWebApi.Application.Interfaces;
using FullWebApi.Domain.Dtos;
using FullWebApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FullWebApi.Api.EndPoints;

public class GetAllUsers : EndpointWithoutRequest<List<UserDto>>
{
  private readonly AppDBContext _context;
  private readonly IUserServices _userServices;
  public GetAllUsers(AppDBContext context, IUserServices userServices)
  {
    _context = context;
    _userServices = userServices;
  }

  public override void Configure()
  {
    Get("/api/user/users");
    Roles("Admin");
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
