using Kiroku.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Persistence
{
    public class KirokuDatabaseContext : DbContext
    {
        public KirokuDatabaseContext(DbContextOptions<KirokuDatabaseContext> options) : base(options) { }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<LogLevel>();

            modelBuilder.Entity<Log>()
               .Property(b => b.Id)
               .UseSerialColumn();

            modelBuilder.Entity<Log>()
                .HasIndex(b => b.Time);
            modelBuilder.Entity<Log>()
                .HasIndex(b => b.ProjectId);
            modelBuilder.Entity<Log>()
                .HasIndex(b => b.EventCode);
            modelBuilder.Entity<Log>()
                .HasIndex(b => b.Level);
        }
    }
}
