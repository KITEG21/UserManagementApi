using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FullWebApi.Domain.Models;
using FullWebApi.Infrastructure.Configurations;
using FullWebApi.Domain.Models.Auth;


namespace FullWebApi.Infrastructure.Data;

public class AppDBContext : DbContext
{
  public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
  {
  }
  public required DbSet<User> Users { get; set; }
  public required DbSet<AdminUser> AdminUsers { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfiguration(new UserConfiguration());
  }
}    
