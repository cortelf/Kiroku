using Kiroku.Business.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.PartitionWorker.Jobs
{
    public class DeletePartitionsJob: IJob
    {
        private readonly ILogger<DeletePartitionsJob> _logger;
        private readonly IPostgresPartitionService _postgresPartitionService;
        public DeletePartitionsJob(ILogger<DeletePartitionsJob> logger, IPostgresPartitionService postgresPartitionService)
        {
            _logger = logger;
            _postgresPartitionService = postgresPartitionService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogDebug("[DeletePartitionsJob.Execute] called");
            await _postgresPartitionService.DeleteOldPartitions();
        }
    }
}
