using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Persistence.Dao
{
    public interface IPostgresPartitionDao
    {
        Task<IEnumerable<string>> GetLogsTablePartitions();
        Task CreateTablePartition(string partitionName, DateTime startDate, DateTime endDate);
        Task DeletePartition(string partitionName);
    }
}
