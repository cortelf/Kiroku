using Kiroku.Business.Impl;
using Kiroku.Business.MappingProfiles;
using Kiroku.Business.Services;
using Kiroku.Business.Services.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Business
{
    public static class BusinessDependencyInjection
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services)
        {
            services.AddSingleton<IPartitionListAnalyzeStrategy, PartitionListAnalyzeStrategy>();
            services.AddSingleton<IPartitionTableNamingStrategy, PartitionTableNamingStrategy>();
            services.AddScoped<IPostgresPartitionService, PostgresPartitionService>();

            services.AddScoped<ILogService, LogService>();
            services.AddAutoMapper(typeof(LogRecordProfile));
            return services;
        }
    }
}
