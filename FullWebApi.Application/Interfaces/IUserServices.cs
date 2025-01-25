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
    public Task<List<UserDto>?> GetAllUsers();
    public Task<User> GetUser(int id);
    public Task<User> SignUpUser(User req);
    public Task<bool> DeleteUser(int id);
    public Task<User> UpdateUser(User user);
  }
