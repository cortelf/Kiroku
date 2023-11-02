using Kiroku.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Business.Services
{
    public interface ILogService
    {
        public Task WriteLog(CreateLogRecord record, CancellationToken cancellationToken = default);
        public Task WriteLogBatch(List<CreateLogRecord> record, CancellationToken cancellationToken = default);
    }
}
