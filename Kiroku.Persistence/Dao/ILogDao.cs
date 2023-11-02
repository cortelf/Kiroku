using Kiroku.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Persistence.Dao
{
    public interface ILogDao
    {
        Task InsertLogs(List<Log> logs, CancellationToken cancellationToken = default);
    }
}
