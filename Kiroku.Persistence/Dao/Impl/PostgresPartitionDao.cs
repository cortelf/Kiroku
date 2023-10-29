using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Persistence.Dao.Impl
{
    public class PostgresPartitionDao : IPostgresPartitionDao
    {
        private readonly KirokuDatabaseContext _databaseContext;
        public PostgresPartitionDao(KirokuDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task CreateTablePartition(string partitionName, DateTime startDate, DateTime endDate)
        {
            await _databaseContext.Database.ExecuteSqlRawAsync($"CREATE TABLE \"{partitionName}\" PARTITION OF logs FOR VALUES FROM ('{startDate:o}') TO ('{endDate:o}');");
        }

        public async Task DeletePartition(string partitionName)
        {
            await _databaseContext.Database.ExecuteSqlRawAsync($"DROP TABLE \"{partitionName}\";");
        }

        public async Task<IEnumerable<string>> GetLogsTablePartitions()
        {
            return await _databaseContext.Database.SqlQueryRaw<string>(@"
SELECT
    child.relname
FROM pg_inherits
    JOIN pg_class parent            ON pg_inherits.inhparent = parent.oid
    JOIN pg_class child             ON pg_inherits.inhrelid   = child.oid
WHERE parent.relname='logs';
            ").ToListAsync();
        }
    }
}
