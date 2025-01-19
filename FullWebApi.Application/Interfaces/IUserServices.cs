using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Domain.Dtos;
using FastEndpoints;
using FullWebApi.Domain.Models;
using FluentValidation;
using LanguageExt;

namespace FullWebApi.Application.Interfaces;
  public interface IUserServices
  {
    public Task<List<User>?> GetAllUsers();
    public Task<UserDto> SignUpUser(UserDto req);
  }
