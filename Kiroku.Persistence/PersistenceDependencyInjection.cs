using Kiroku.Persistence.Dao;
using Kiroku.Persistence.Dao.Impl;
using Kiroku.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Kiroku.Persistence
{
    public static class PersistenceDependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            AddDatabase(services, configuration);
            AddDao(services, configuration);
            return services;
        }

        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Primary") 
                ?? throw new Exception("Primary connection string not found");

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.MapEnum<LogLevel>();
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<KirokuDatabaseContext>(options =>
                    options.UseNpgsql(dataSource,
                        opt => opt.MigrationsAssembly(typeof(KirokuDatabaseContext).Assembly.FullName)));
        }

        public static void AddDao(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IPostgresPartitionDao, PostgresPartitionDao>();
            services.AddScoped<ILogDao, LogDao>();
        }
    }
}
