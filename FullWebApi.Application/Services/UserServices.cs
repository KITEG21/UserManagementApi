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
    User newUser = _mapper.UserDtoToUser(req);

    //Saving the newUser into DB
    await _context.Users.AddAsync(newUser);
    await _context.SaveChangesAsync();     
            
    //Creating a userDto to return the info
    return _mapper.UserToUserDto(newUser);
           
  }
}
