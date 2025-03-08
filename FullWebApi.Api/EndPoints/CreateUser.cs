using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FluentValidation.Results;
using FullWebApi.Application.Interfaces;
using FullWebApi.Application.Services;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Models;
using FullWebApi.Domain.ModelsValidator;
using FullWebApi.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FullWebApi.Api.EndPoints;
public class CreateUser : Endpoint<User>
{
  private readonly IUserRepository _userRepository;
  private readonly IUserServices _userServices;
  private readonly AppDBContext _context;

  public CreateUser(AppDBContext context, IUserRepository userRepository, IUserServices userServices)
  {
    _context = context;
    _userRepository = userRepository;
    _userServices = userServices;
  }

  public override void Configure()
  {
    Post("/api/user/signup");
    Roles("Admin");
  }

  public override async Task HandleAsync(User req, CancellationToken ct)
  { 
    if(await _context.Users.AnyAsync(u => u.Username == req.Username && u.Email == req.Email))
    {
      var errorResponse = new 
      {
        StatusCode = 409,
        Errors = "User already exist"
      };

      await SendAsync(errorResponse, 409, ct);
      Log.Information("Create user attemp failed: User already exist. Username: {username} Email: {email}", req.Username, req.Email);
      return;
    }
    var newUser = await _userServices.SignUpUser(req);
    if(newUser.User == null){ 
      await SendAsync(newUser.ErrorResponse, 400, ct);
      return;
    }

    await SendAsync(newUser, 201, ct);
    return;
  }  
}
