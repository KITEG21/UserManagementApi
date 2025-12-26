using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Domain.Enums;

namespace FullWebApi.Domain.Dtos;

public class UserDto
{ 
  public Guid Id { get; set; }  
  public string Username { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public UserStatus Status { get; set; }
  public string StatusName => Status.ToString();
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public DateTime? LastLoginAt { get; set; }
  public bool IsActive { get; set; }
}
