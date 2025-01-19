using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Models;
using Riok.Mapperly;
using Riok.Mapperly.Abstractions;

namespace FullWebApi.Application.Mappings;

[Mapper]
public partial class UserMapper
{
  public partial UserDto UserToUserDto(User user);
  public partial User UserDtoToUser(UserDto userDto);
}
