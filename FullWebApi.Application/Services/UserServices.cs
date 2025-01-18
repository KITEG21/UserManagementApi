using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using FullWebApi.Application.Interfaces;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Models;
using FullWebApi.Domain.ModelsValidator;
using FullWebApi.Infrastructure.Data;
using LanguageExt;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullWebApi.Application.Services;

public class UserServices : IUserServices
{
  private readonly AppDBContext _context;
  public UserServices(AppDBContext context)
  {
    _context = context;
  }

  public async Task<List<User>?> GetAllUsers()
  {
    var users = await _context.Users.ToListAsync(); 
           
    if(users == null){
      return null;
    }   
  return users;
  }

  public async Task<UserDto> SignUpUser(UserDto req)
  {    
    //Creation of new user for the DB
    User newUser = new User
    {
      Name = req.Name,
      Email = req.Email,
      Password = req.Password
    };

    //Saving the newUser into DB
    await _context.Users.AddAsync(newUser);
    await _context.SaveChangesAsync();     
            
    //Creating a userDto to return the info
    return new UserDto()
    {
      Name = newUser.Name,
      Email = newUser.Email,
      Password = newUser.Password
    };            
  }
}
