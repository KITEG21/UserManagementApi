using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using FullWebApi.Application.Interfaces;
using FullWebApi.Application.Mappings;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Models;
using FullWebApi.Domain.ModelsValidator;
using FullWebApi.Infrastructure.Data;
using LanguageExt;
using LanguageExt.Pipes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullWebApi.Application.Services;

public class UserServices : IUserServices
{
  private readonly AppDBContext _context;
  private readonly UserMapper _mapper;

  public UserServices(AppDBContext context, UserMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<List<UserDto>?> GetAllUsers()
  {
    var users = await _context.Users.ToListAsync(); 

    var usersList = users.Select(user => _mapper.UserToUserDto(user)).ToList();

    if(users == null){
      return null;
    }   
    return usersList;
  }

    public async Task<User> GetUser(int id)
    {
      User? user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("User not found");
      return user;
    }

    public async Task<User> SignUpUser(User req)
  {    
    //Creation of new user for the DB
    User newUser = req;

    //Saving the newUser into DB
    await _context.Users.AddAsync(newUser);
    await _context.SaveChangesAsync();     
            
    //Creating a userDto to return the info
    return newUser;         
  }

// Delete user by ID
  public async Task<bool> DeleteUser(int id)
  {
    var user = await _context.Users.FindAsync(id);
    if (user == null)
    {
      return false;
    }

    _context.Users.Remove(user);
    await _context.SaveChangesAsync();
    return true;
    }

  // Update user details
  public async Task<User> UpdateUser(User req)
  {
    var updatedUser = await _context.Users.FirstOrDefaultAsync(x=> x.Id == req.Id);
   
    if (updatedUser == null)
    {
      return null;
    }

   updatedUser.Name = req.Name;
   updatedUser.Email = req.Email;
   updatedUser.Password = req.Password;

    await _context.SaveChangesAsync();
    return updatedUser;
  }  

}
