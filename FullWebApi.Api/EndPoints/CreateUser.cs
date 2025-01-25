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

    if(!validationResult.IsValid)
    {
      var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
      var errorResponse = new
      {
        StatusCode = 400,
        Errors = errorMessages
      };

      await SendAsync(errorResponse, 400, ct);
      return;
    }
    else
    {
      var newUser = await _userServices.SignUpUser(req);
      await SendAsync(newUser, 201, ct);
    }
  }  
}
