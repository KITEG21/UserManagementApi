using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Domain.Models.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FullWebApi.Infrastructure.Configurations;

public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x=> x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Username)
        .IsRequired()
        .HasMaxLength(100);

        builder.Property(x => x.Email)
        .IsRequired()
        .HasMaxLength(100);

        builder.Property(x => x.Password).IsRequired();

    }
}
