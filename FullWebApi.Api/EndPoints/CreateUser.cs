using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FullWebApi.Application.Interfaces;
using FullWebApi.Application.Mappings;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Models;
using FullWebApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FullWebApi.Api.EndPoints;

public class CreateUser : Endpoint<CreateUserDto>
{
  private readonly IUserServices _userServices;
  private readonly AppDBContext _context;
  private readonly UserMapper _mapper;

  public CreateUser(AppDBContext context, IUserServices userServices, UserMapper mapper)
  {
    _context = context;
    _userServices = userServices;
    _mapper = mapper;
  }

  public override void Configure()
  {
    Post("/api/user/post");
    Roles("Admin");
    Tags("Users");
    Summary(s =>
    {
      s.Summary = "Create a new user";
      s.Description = "Creates a new user with PendingVerification status. Only Username, Email, and Password are required.";
    });
  }

  public override async Task HandleAsync(CreateUserDto req, CancellationToken ct)
  { 
    if (await _context.Users.AnyAsync(u => u.Username == req.Username || u.Email == req.Email, ct))
    {
      Log.Information("Create user failed: User already exists. Username: {username} Email: {email}", req.Username, req.Email);
      ThrowError("User already exists", 409);
    }
    
    var user = _mapper.CreateUserDtoToUser(req);
    var result = await _userServices.SignUpUser(user);
    
    if (result.User == null)
    { 
      Log.Information("Create user failed: Validation errors");
      ThrowError(result.ErrorResponse?.ToString() ?? "Failed to create user", 400);
    }

    Log.Information("User created successfully. Username: {username}", result.User.Username);
    var userDto = _mapper.UserToUserDto(result.User);
    await Send.CreatedAtAsync<GetUserById>(new { id = result.User.Id }, userDto, cancellation: ct);
  }  
}
