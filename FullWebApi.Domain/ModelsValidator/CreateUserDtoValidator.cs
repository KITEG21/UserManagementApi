using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FullWebApi.Domain.Dtos;

namespace FullWebApi.Domain.ModelsValidator;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
  public CreateUserDtoValidator()
  {
    RuleFor(x => x.Username)
      .NotEmpty().WithMessage("Username is required")
      .MinimumLength(3).WithMessage("Username must be at least 3 characters")
      .MaximumLength(50).WithMessage("Username must not exceed 50 characters");

    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required")
      .EmailAddress().WithMessage("Invalid email format")
      .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

    RuleFor(x => x.Password)
      .NotEmpty().WithMessage("Password is required")
      .MinimumLength(6).WithMessage("Password must be at least 6 characters")
      .MaximumLength(100).WithMessage("Password must not exceed 100 characters");
  }
}
