using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using FluentValidation.Results;
using FullWebApi.Application.Interfaces;
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
  private readonly AppDBContext _context;
  private readonly IUserServices _userServices;

  public CreateUser(AppDBContext context, IUserServices userServices)
  {
    _context = context;
    _userServices = userServices;
  }

  public override void Configure()
  {
    Post("/api/user/signup");
    Roles("Admin");
  }

  public override async Task HandleAsync(User req, CancellationToken ct)
  { 
    UserValidator userValidator = new();
    ValidationResult validationResult = userValidator.Validate(req);

    if(await _context.Users.AnyAsync(u => u.Username == req.Username && u.Email == req.Email))
    {
      var errorResponse = new 
      {
        StatusCode = 409,
        Errors = "User already exist"
      };

      await SendAsync(errorResponse, 409, ct);
      Log.Information("Create user attemp failed: User already exist. Username: {username} Email: {email}", req.Username, req.Email);
    }


    if(!validationResult.IsValid)
    {
      var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
      var errorResponse = new
      {
        StatusCode = 400,
        Errors = errorMessages
      };

      await SendAsync(errorResponse, 400, ct);
      Log.Information("Create user attempt failed: Invalid credentials for new user");
      return;
    }
    else
    {
      var newUser = await _userServices.SignUpUser(req);
      await SendAsync(newUser, 201, ct);
      Log.Information("New user created successfully! Username: {username}, Email: {email}", newUser.Username, newUser.Email);
    }
  }  
}
