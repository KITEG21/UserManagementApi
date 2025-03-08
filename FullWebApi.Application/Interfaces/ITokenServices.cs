using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Domain.Models.Auth;

namespace FullWebApi.Application.Interfaces;

public interface ITokenServices
{
  string GenerateToken(string username, string role);  

}
