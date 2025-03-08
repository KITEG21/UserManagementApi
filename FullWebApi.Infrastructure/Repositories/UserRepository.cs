using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Application.Interfaces;
using FullWebApi.Application.Mappings;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Models;
using FullWebApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace FullWebApi.Infrastructure.Repositories;

internal class UserRepository : IUserRepository
{
  private readonly AppDBContext _context;
  private readonly UserMapper _mapper;
  private readonly IMemoryCache _cache;
  public UserRepository(AppDBContext context, UserMapper mapper, IMemoryCache cache)
  {
    _context = context;
    _mapper = mapper;
    _cache = cache;
  }

    public async Task<List<UserDto>?> GetAllUsers()
  {
    var cacheKey = "allUsers";
    if(_cache.TryGetValue(cacheKey, out List<UserDto>? cachedUsers)){
      Log.Information("Cache hit for key: {CacheKey}", cacheKey);
      return cachedUsers;
    }

    Log.Information("Cache miss for key: {CacheKey}", cacheKey);

    var users = await _context.Users.ToListAsync(); 
    var usersList = users.Select(user => _mapper.UserToUserDto(user)).ToList();

    if(users == null){
      return null;
    }   

    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
      .SetSlidingExpiration(TimeSpan.FromMinutes(5))
      .SetAbsoluteExpiration(TimeSpan.FromMinutes(60));

    _cache.Set(cacheKey, usersList, cacheEntryOptions);
    Log.Information("Cache set for key: {CacheKey}", cacheKey);

    return usersList;
  }

  public async Task<User> GetUser(int id)
  {
    var cacheKey = $"user{id}";
    if(_cache.TryGetValue(cacheKey, out User? cachedUser) && cachedUser != null){
      Log.Information("Cache hit for key: {CacheKey}", cacheKey);
      return cachedUser;
    }

    Log.Information("Cache miss for key: {CacheKey}", cacheKey);
    User? user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("User not found");
    if(user == null){
      return null;
    }
    
    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions() 
      .SetSlidingExpiration(TimeSpan.FromMinutes(5))
      .SetAbsoluteExpiration(TimeSpan.FromMinutes(60));

    _cache.Set(cacheKey, user, cacheEntryOptions);
    Log.Information("Cache set for key: {CacheKey}", cacheKey);
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
    if (user == null){
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
   
    if (updatedUser == null){
      throw new Exception("User not found");
    }

   updatedUser.Username = req.Username;
   updatedUser.Email = req.Email;
   updatedUser.Password = req.Password;

    await _context.SaveChangesAsync();     
    return updatedUser;
  }  
}
