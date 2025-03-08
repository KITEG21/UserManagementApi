using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Application.Interfaces;
using FullWebApi.Infrastructure.Data;
using FullWebApi.Infrastructure.Repositories;

namespace FullWebApi.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDBContext _context;
    public IUserRepository _userRepository { get; }

  public UnitOfWork(AppDBContext context, IUserRepository userRepository)
  {
    _context = context;
    _userRepository = userRepository;
  }


    public void Dispose()
  {
    _context.Dispose();
  }

  public async Task<int> SaveChangesAsync()
  {
    return await _context.SaveChangesAsync();
  }

}
