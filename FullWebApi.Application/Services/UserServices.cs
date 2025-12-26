using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation.Results;
using FullWebApi.Application.Interfaces;
using FullWebApi.Application.Mappings;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Enums;
using FullWebApi.Domain.Models;
using FullWebApi.Domain.ModelsValidator;
using FluentValidation;
using Serilog;

namespace FullWebApi.Application.Services;

public class UserServices : IUserServices
{
  private readonly IUnitOfWork _unitOfWork;

  public UserServices(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<bool> DeleteUser(Guid id)
  {
    Log.Information("Attempting to delete user with ID: {id}", id);

    var result = await _unitOfWork._userRepository.DeleteUser(id);
    if (!result)
    {
      Log.Warning("Failed to delete user with ID: {id}", id);
    }
    return result;
  }

  public async Task<List<UserDto>> GetAllUsers()
  {
    Log.Information("Getting all users");
    var users = await _unitOfWork._userRepository.GetAllUsers();

    if(users != null && users.Any()){
      Log.Information("All users retrieved successfully");
    }
    else{
      Log.Information("No users found");
    }
    return users ?? new List<UserDto>();
  }

  public async Task<PaginatedResponse<UserDto>> GetUsersAsync(UserSearchRequest request)
  {
    Log.Information("Getting users with pagination. Page: {Page}, PageSize: {PageSize}", request.Page, request.PageSize);
    
    var (users, totalCount) = await _unitOfWork._userRepository.GetUsersAsync(request);
    
    var response = new PaginatedResponse<UserDto>
    {
      Data = users,
      Page = request.Page,
      PageSize = request.PageSize,
      TotalCount = totalCount,
      TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
    };

    Log.Information("Retrieved {Count} users out of {Total} total", users.Count, totalCount);
    return response;
  }

  public async Task<User?> GetUser(Guid id)
  {
    Log.Information("Searching user with id: {id}", id);
    var user = await _unitOfWork._userRepository.GetUser(id);

    if(user != null){
      Log.Information("User found with id: {id}", id);
    }
    else{
      Log.Information("User not found with id: {id}", id);
    }
    return user;
  }

  public async Task<(User? User, object? ErrorResponse)> SignUpUser(User req)
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
      Log.Information("Create user attempt failed: Invalid credentials for new user");
      return (null, errorResponse);
    }
    
    var newUser = await _unitOfWork._userRepository.SignUpUser(req);
    if(newUser == null){
      var errorResponse = new
      {
        StatusCode = 400,
        Errors = "Failed to create a new user"
      };
      Log.Information("Create user attempt failed: Failed to create a new user");
      return (null, errorResponse);
    }
    Log.Information("New user created. Username: {username}", newUser.Username);
    return (newUser, null);
  }

  public async Task<User?> UpdateUser(User user)
  {
    Log.Information("Attempting to update user with ID: {id}", user.Id);
    var updatedUser = await _unitOfWork._userRepository.UpdateUser(user);

    if(updatedUser == null){
      Log.Warning("Failed to update user with ID: {id}", user.Id);
    }
    return updatedUser;
  }

  public async Task<bool> UpdateUserStatus(Guid userId, UserStatus status, string? reason)
  {
    Log.Information("Updating user {UserId} status to {Status}", userId, status);
    return await _unitOfWork._userRepository.UpdateUserStatus(userId, status, reason);
  }

  public async Task<bool> ActivateUser(Guid userId)
  {
    Log.Information("Activating user {UserId}", userId);
    return await UpdateUserStatus(userId, UserStatus.Active, null);
  }

  public async Task<bool> SuspendUser(Guid userId, string? reason)
  {
    Log.Information("Suspending user {UserId}. Reason: {Reason}", userId, reason);
    return await UpdateUserStatus(userId, UserStatus.Suspended, reason);
  }

  public async Task<bool> DeactivateUser(Guid userId, string? reason)
  {
    Log.Information("Deactivating user {UserId}. Reason: {Reason}", userId, reason);
    return await UpdateUserStatus(userId, UserStatus.Inactive, reason);
  }
}