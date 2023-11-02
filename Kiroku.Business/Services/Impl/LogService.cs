using AutoMapper;
using Kiroku.Business.Exceptions;
using Kiroku.Business.Models;
using Kiroku.Persistence;
using Kiroku.Persistence.Dao;
using Kiroku.Persistence.Entities;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kiroku.Business.Services.Impl
{
    public class LogService : ILogService
    {
        private readonly ILogDao _logDao;
        private readonly IMapper _mapper;

        public LogService(ILogDao logDao, IMapper mapper)
        {
            _logDao = logDao;
            _mapper = mapper;
        }

        public Task WriteLog(CreateLogRecord record, CancellationToken cancellationToken = default)
        {
            return WriteLogBatch(new List<CreateLogRecord> { record }, cancellationToken);
        }

        public async Task WriteLogBatch(List<CreateLogRecord> record, CancellationToken cancellationToken = default)
        {
            if (!record.Any())
                return;

            foreach (var item in record)
            {
                if (Math.Abs((DateTime.UtcNow - item.Timestamp).TotalHours) > 24)
                    throw new BadLogRecordDateTimeException();
            }

            var logs = _mapper.Map<List<CreateLogRecord>, List<Log>>(record);

            await _logDao.InsertLogs(logs, cancellationToken);
        }
    }
}
