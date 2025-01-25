using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FullWebApi.Domain.Models.Auth;

namespace FullWebApi.Domain.ModelsValidator;

public class SignUpRequestValidator : AbstractValidator<SignUpRequest>
{
  public SignUpRequestValidator()
  {
    RuleFor(x => x.Username)
      .NotEmpty().WithMessage("Username is required")
      .MinimumLength(3).WithMessage("Username cannot have less than 3 characters")
      .MaximumLength(100).WithMessage("Username cannot have more than 100 characters");

    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required")
      .EmailAddress().WithMessage("Email not valid")
      .MaximumLength(100).WithMessage("Email cannot have more than 100 characters");

    RuleFor(x => x.Password)
      .NotEmpty().WithMessage("Password is required")
      .MinimumLength(6).WithMessage("Password must have more than 6 characters"); 
   }
}
