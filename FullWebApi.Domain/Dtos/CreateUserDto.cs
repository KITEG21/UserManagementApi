using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullWebApi.Domain.Dtos;

public class CreateUserDto
{
  public string Username { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}
