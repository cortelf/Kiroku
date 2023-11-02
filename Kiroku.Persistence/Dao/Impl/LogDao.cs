using Kiroku.Persistence.Entities;
using Kiroku.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Persistence.Dao.Impl
{
    public class LogDao : ILogDao
    {
        private readonly KirokuDatabaseContext _databaseContext;

        public LogDao(KirokuDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task InsertLogs(List<Log> logs, CancellationToken cancellationToken = default)
        {
            try
            {
                await _databaseContext.AddRangeAsync(logs, cancellationToken: cancellationToken);
                await _databaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
            }
            catch (DbUpdateException e)
                when (e.InnerException is PostgresException 
                    && (e.InnerException as PostgresException)!.SqlState == "23514")
            {
                throw new NoPartitionException(e.Message, e);
            }
            catch (DbUpdateException e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }
    }
}
