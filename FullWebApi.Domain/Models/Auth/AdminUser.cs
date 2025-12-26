using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullWebApi.Domain.Models.Auth;

public class AdminUser
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
