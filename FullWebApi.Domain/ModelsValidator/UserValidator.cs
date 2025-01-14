using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Models;

namespace FullWebApi.Domain.ModelsValidator
{
    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name can't be longer than 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .MaximumLength(100).WithMessage("Email can't be longer than 100 characters")
                .EmailAddress().WithMessage("Email is not valid");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
        }

    }
        
}