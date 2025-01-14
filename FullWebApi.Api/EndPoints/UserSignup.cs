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

namespace FullWebApi.Api.EndPoints
{
    public class UserSignup : Endpoint<UserDto, object>
    {
        private readonly AppDBContext _context;
        private readonly IUserServices _userServices;

        public UserSignup(AppDBContext context, IUserServices userServices)
        {
          _context = context;
          _userServices = userServices;
        }

        public override void Configure()
        {
          Post("/api/user/signup");
          AllowAnonymous();
        }

        public override async Task HandleAsync(UserDto req, CancellationToken ct)
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
              await SendAsync(errorResponse);
              return;
            }
            else
            {
              var newUser = _userServices.SignUpUser(req);
              await SendAsync(newUser, 201, cancellation: ct);
            }
      }  
    }
}