using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Application.Interfaces;
using FullWebApi.Application.Mappings;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Enums;
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

  public async Task<(List<UserDto> Users, int TotalCount)> GetUsersAsync(UserSearchRequest request)
  {
    var query = _context.Users.AsQueryable();

    // Apply search filter
    if (!string.IsNullOrWhiteSpace(request.SearchTerm))
    {
      var searchTerm = request.SearchTerm.ToLower();
      query = query.Where(u => 
        u.Username.ToLower().Contains(searchTerm) || 
        u.Email.ToLower().Contains(searchTerm));
    }

    // Apply status filter
    if (request.Status.HasValue)
    {
      query = query.Where(u => u.Status == request.Status.Value);
    }

    // Apply date filters
    if (request.CreatedAfter.HasValue)
    {
      query = query.Where(u => u.CreatedAt >= request.CreatedAfter.Value);
    }

    if (request.CreatedBefore.HasValue)
    {
      query = query.Where(u => u.CreatedAt <= request.CreatedBefore.Value);
    }

    // Get total count before pagination
    var totalCount = await query.CountAsync();

    // Apply sorting
    query = request.SortBy?.ToLower() switch
    {
      "username" => request.SortOrder?.ToLower() == "asc" 
        ? query.OrderBy(u => u.Username) 
        : query.OrderByDescending(u => u.Username),
      "email" => request.SortOrder?.ToLower() == "asc" 
        ? query.OrderBy(u => u.Email) 
        : query.OrderByDescending(u => u.Email),
      "status" => request.SortOrder?.ToLower() == "asc" 
        ? query.OrderBy(u => u.Status) 
        : query.OrderByDescending(u => u.Status),
      _ => request.SortOrder?.ToLower() == "asc" 
        ? query.OrderBy(u => u.CreatedAt) 
        : query.OrderByDescending(u => u.CreatedAt)
    };

    // Apply pagination
    var users = await query
      .Skip((request.Page - 1) * request.PageSize)
      .Take(request.PageSize)
      .ToListAsync();

    var userDtos = users.Select(user => _mapper.UserToUserDto(user)).ToList();

    Log.Information("Retrieved {Count} users out of {Total} total", userDtos.Count, totalCount);
    return (userDtos, totalCount);
  }

  public async Task<User?> GetUser(Guid id)
  {
    var cacheKey = $"user{id}";
    if(_cache.TryGetValue(cacheKey, out User? cachedUser) && cachedUser != null){
      Log.Information("Cache hit for key: {CacheKey}", cacheKey);
      return cachedUser;
    }

    Log.Information("Cache miss for key: {CacheKey}", cacheKey);
    User? user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
    if(user == null){
      Log.Information("User not found with id: {id}", id);
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
    req.Id = Guid.NewGuid();
    req.CreatedAt = DateTime.UtcNow;
    req.Status = UserStatus.PendingVerification;
    
    await _context.Users.AddAsync(req);
    await _context.SaveChangesAsync();     
            
    // Clear cache when new user is added
    _cache.Remove("allUsers");
    
    return req;         
  }

  public async Task<bool> DeleteUser(Guid id)
  {
    var user = await _context.Users.FindAsync(id);
    if (user == null){
      return false;
    }

    _context.Users.Remove(user);
    await _context.SaveChangesAsync();
    
    // Clear cache
    _cache.Remove("allUsers");
    _cache.Remove($"user{id}");
    
    return true;
  }

  public async Task<User?> UpdateUser(User req)
  {
    var updatedUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == req.Id);
   
    if (updatedUser == null){
      Log.Information("User not found with id: {id}", req.Id);
      return null;
    }

    updatedUser.Username = req.Username;
    updatedUser.Email = req.Email;
    updatedUser.Password = req.Password;
    updatedUser.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    
    // Clear cache
    _cache.Remove("allUsers");
    _cache.Remove($"user{req.Id}");
    
    return updatedUser;
  }

  public async Task<bool> UpdateUserStatus(Guid userId, UserStatus status, string? reason)
  {
    var user = await _context.Users.FindAsync(userId);
    if (user == null)
    {
      Log.Information("User not found with id: {id}", userId);
      return false;
    }

    user.Status = status;
    user.StatusReason = reason;
    user.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    
    // Clear cache
    _cache.Remove("allUsers");
    _cache.Remove($"user{userId}");
    
    Log.Information("User {UserId} status updated to {Status}", userId, status);
    return true;
  }
}
