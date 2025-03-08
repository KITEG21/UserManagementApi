using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullWebApi.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository _userRepository {get;}
        Task<int> SaveChangesAsync();
    }
}