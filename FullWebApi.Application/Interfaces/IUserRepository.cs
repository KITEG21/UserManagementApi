using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Enums;
using FullWebApi.Domain.Models;

namespace FullWebApi.Application.Interfaces;

public interface IUserRepository
{
    Task<List<UserDto>?> GetAllUsers();
    Task<(List<UserDto> Users, int TotalCount)> GetUsersAsync(UserSearchRequest request);
    Task<User?> GetUser(Guid id);
    Task<User> SignUpUser(User req);
    Task<bool> DeleteUser(Guid id);
    Task<User?> UpdateUser(User user);
    Task<bool> UpdateUserStatus(Guid userId, UserStatus status, string? reason);
}
