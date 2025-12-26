using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Domain.Dtos;
using FastEndpoints;
using FullWebApi.Domain.Models;
using FullWebApi.Domain.Enums;
using FluentValidation;
using LanguageExt;

namespace FullWebApi.Application.Interfaces;

public interface IUserServices
{
    Task<List<UserDto>> GetAllUsers();
    Task<PaginatedResponse<UserDto>> GetUsersAsync(UserSearchRequest request);
    Task<User?> GetUser(Guid id);
    Task<(User? User, object? ErrorResponse)> SignUpUser(User req);
    Task<bool> DeleteUser(Guid id);
    Task<User?> UpdateUser(User user);
    Task<bool> UpdateUserStatus(Guid userId, UserStatus status, string? reason);
    Task<bool> ActivateUser(Guid userId);
    Task<bool> SuspendUser(Guid userId, string? reason);
    Task<bool> DeactivateUser(Guid userId, string? reason);
}
