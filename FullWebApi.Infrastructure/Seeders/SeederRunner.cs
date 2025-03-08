using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullWebApi.Application.Interfaces;

namespace FullWebApi.Infrastructure.Seeders;

public class SeederRunner
{
    private readonly IEnumerable<ISeeder> _seeders;
    
    public SeederRunner(IEnumerable<ISeeder> seeders)
    {
        _seeders = seeders;
    }

    public async Task RunAsync()
    {
        foreach (var seeder in _seeders)
        {
            await seeder.SeedAsync();
        }
    }


}
