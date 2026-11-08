using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ScadaWebUI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SensorSample> SensorSamples => Set<SensorSample>();
}