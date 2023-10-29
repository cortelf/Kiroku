using Kiroku.Persistence.Dao;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Business.Services.Impl
{
    public class PostgresPartitionService : IPostgresPartitionService
    {
        private readonly IPartitionListAnalyzeStrategy _analyzeStrategy;
        private readonly IPartitionTableNamingStrategy _namingStrategy;
        private readonly ILogger<PostgresPartitionService> _logger;
        private readonly IPostgresPartitionDao _partitionDao;

        public PostgresPartitionService(
            IPartitionListAnalyzeStrategy analyzeStrategy, IPartitionTableNamingStrategy namingStrategy, 
            ILogger<PostgresPartitionService> logger, IPostgresPartitionDao partitionDao)
        {
            _analyzeStrategy = analyzeStrategy;
            _namingStrategy = namingStrategy;
            _logger = logger;
            _partitionDao = partitionDao;
        }

        public async Task<CreateNextPartitionResult> CreateNextPartition()
        {
            _logger.LogDebug("[CreateNextPartition] called");
            var partitionNames = await _partitionDao.GetLogsTablePartitions();
            var partitionDates = partitionNames.Select(x => _namingStrategy.ParsePartitionName(x));
            var lastDate = _analyzeStrategy.GetLastPartitionsDate(partitionDates);
            _logger.LogDebug("[CreateNextPartition] Last date={lastDate}", lastDate);

            if (!_analyzeStrategy.CheckCreatingNewPartitionIsRequired(lastDate))
            {
                _logger.LogDebug("[CreateNextPartition] No need");
                return CreateNextPartitionResult.NoNeed;
            }

            var toDate = lastDate.AddDays(1);
            var createdPartitionName = _namingStrategy.MakePartitionName(toDate);

            await _partitionDao.CreateTablePartition(createdPartitionName,
                lastDate.ToDateTime(new TimeOnly(), DateTimeKind.Utc), 
                toDate.ToDateTime(new TimeOnly(), DateTimeKind.Utc));
            _logger.LogInformation("[CreateNextPartition] Created new partition with name {partitionName} from {fromDate} to {toDate}", 
                createdPartitionName, lastDate, toDate);

            return CreateNextPartitionResult.Created;
        }

        public async Task DeleteOldPartitions()
        {
            _logger.LogDebug("[DeleteOldPartitions] called");
            var partitionNames = await _partitionDao.GetLogsTablePartitions();
            var partitionDates = partitionNames.Select(x => _namingStrategy.ParsePartitionName(x));

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var expiredDates = partitionDates.Where(x => today.DayNumber - x.DayNumber >= 7);
            var partitionsToDeleteNames = expiredDates.Select(x => _namingStrategy.MakePartitionName(x));
            _logger.LogDebug("[DeleteOldPartitions] {deleteItemsCount} partitions to delete", partitionsToDeleteNames.Count());

            foreach ( var partitionToDelete in partitionsToDeleteNames)
            {
                await _partitionDao.DeletePartition(partitionToDelete);
                _logger.LogInformation("[DeleteOldPartitions] The {partitionName} partition has been removed", partitionToDelete);
            }

        }
    }
}
