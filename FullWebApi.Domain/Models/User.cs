using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Domain.Enums;

namespace FullWebApi.Domain.Models;

public class User
{
  public Guid Id { get; set; }
  public string Username { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public UserStatus Status { get; set; } = UserStatus.PendingVerification;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
  public DateTime? LastLoginAt { get; set; }
  public string? StatusReason { get; set; }
  public bool IsActive => Status == UserStatus.Active;
}
