using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation.Results;
using FullWebApi.Application.Interfaces;
using FullWebApi.Application.Mappings;
using FullWebApi.Domain.Dtos;
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

    public async Task<bool> DeleteUser(int id)
    {
      Log.Information("Attempting to delete user with ID: {id}", id);

      var result = await _unitOfWork._userRepository.DeleteUser(id);
      if (!result)
      {
        Log.Warning($"Failed to delete user with ID: {id}");
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
      return users;
    }

    public async Task<User> GetUser(int id)
    {
      Log.Information("Searching user with id: {id}",id);
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
      else
      {
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
        Log.Information("New user created. Username : {username}", newUser.Username);
        return (newUser, null);
      }
    }

    public Task<User> UpdateUser(User user)
    {
      Log.Information("Attempting to update user with ID: {id}", user.Id);
      var updatedUser = _unitOfWork._userRepository.UpdateUser(user);

      if(updatedUser == null){
        Log.Warning("Failed to update user with ID: {id}", user.Id);
      }
      return updatedUser;
    }
}
