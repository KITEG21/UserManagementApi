using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Application.Interfaces;
using FullWebApi.Domain.Models.Auth;
using FullWebApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FullWebApi.Infrastructure.Seeders;

public class UserSeeder : ISeeder
{   
  private readonly AppDBContext _context;

  public UserSeeder(AppDBContext context)
  {
    _context = context;
  }

  public async Task SeedAsync()
  {
    if(!await _context.AdminUsers.AnyAsync()){ 

      var adminUser = new List<AdminUser>
      {
        new AdminUser
        {
          Id = Guid.NewGuid(),
          Username = "admin",
          Email = "admin@localhost",
          Password = "admin",
        }
      };

      await _context.AdminUsers.AddRangeAsync(adminUser);
      await _context.SaveChangesAsync();
    }
  }
}
