using Kiroku.Business.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.PartitionWorker.Jobs
{
    public class CreatePartitionJob : IJob
    {
        private readonly ILogger<CreatePartitionJob> _logger;
        private readonly IPostgresPartitionService _postgresPartitionService;
        public CreatePartitionJob(ILogger<CreatePartitionJob> logger, IPostgresPartitionService postgresPartitionService)
        { 
            _logger = logger;
            _postgresPartitionService = postgresPartitionService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogDebug("[CreatePartitionJob.Execute] called");
            await _postgresPartitionService.CreateNextPartition();
        }
    }
}
