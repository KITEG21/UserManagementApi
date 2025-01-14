using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FullWebApi.Domain.Models;

namespace FullWebApi.Infrastructure.Data
{
   public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        public required DbSet<User> Users { get; set; }
    }    
}