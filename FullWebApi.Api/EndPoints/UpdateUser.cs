using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FullWebApi.Application.Interfaces;
using FullWebApi.Domain.Models;
using FullWebApi.Infrastructure.Data;
using Serilog;

namespace FullWebApi.Api.EndPoints;

public class UpdateUser : Endpoint<User>
{
  private readonly AppDBContext _context;
  private readonly IUserServices _userServices;

  public UpdateUser(AppDBContext context, IUserServices userServices)
  {
    _context = context;
    _userServices = userServices;
  }

  public override void Configure()
  {
    Put("api/user/updateuser/{id}");
    Roles("Admin");
  }

  public override async Task HandleAsync(User req, CancellationToken ct)
  {
    var id = Route<int>("id");

    var userUpdate = await _userServices.UpdateUser(req);

    if(userUpdate == null)
    {
        var errorResponse = new 
        {
            StatusCode = 404,
            Error = "The required user doesn't exist"
        };

        await SendAsync(errorResponse, 404, ct);
        Log.Information("Update user failed attempt: The required user Id coulnd't be find. Id: {id}", req.Id);
        return;
    }
    
    await SendAsync(userUpdate);

  }







}
