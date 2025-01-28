using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FullWebApi.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    //Sets the primary key
    builder.HasKey(x => x.Id);

    #region Properties settings

    builder.Property(x => x.Id).ValueGeneratedOnAdd();

    builder.Property(x => x.Username)
    .IsRequired()
    .HasMaxLength(100);

    builder.Property(x => x.Email)
    .IsRequired()
    .HasMaxLength(100);

    builder.Property(x => x.Password).IsRequired();
    
    #endregion
  }     
}
