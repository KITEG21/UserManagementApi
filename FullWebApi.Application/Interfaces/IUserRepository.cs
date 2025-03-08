using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Models;

namespace FullWebApi.Application.Interfaces;

    public interface IUserRepository
    {
    public Task<List<UserDto>?> GetAllUsers();
    public Task<User> GetUser(int id);
    public Task<User> SignUpUser(User req);
    public Task<bool> DeleteUser(int id);
    public Task<User> UpdateUser(User user);
    }
