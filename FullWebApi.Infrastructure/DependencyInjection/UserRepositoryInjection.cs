using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Application.Interfaces;
using FullWebApi.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FullWebApi.Infrastructure.DependencyInjection;

public static class UserRepositoryInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services){
    services.AddScoped<IUserRepository, UserRepository>();
    return services;
  }
}
