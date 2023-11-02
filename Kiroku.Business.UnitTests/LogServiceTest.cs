using AutoMapper;
using Kiroku.Business.Exceptions;
using Kiroku.Business.Models;
using Kiroku.Business.Services.Impl;
using Kiroku.Persistence.Dao;
using Kiroku.Persistence.Entities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text.Json;

namespace Kiroku.Business.UnitTests
{
    [TestClass]
    public class LogServiceTest
    {
        private ILogDao? _logDao;
        private IMapper? _mapper;

        private LogService? _service;

        [TestInitialize]
        public void TestInitialize()
        {
            _logDao = Substitute.For<ILogDao>();
            _mapper = Substitute.For<IMapper>();
            _service = new LogService(_logDao, _mapper);
        }

        [TestMethod]
        public async Task WriteLogBatch_OldDateTime_BadLogRecordDateTimeException()
        {
            var oldDate = DateTime.UtcNow.AddDays(-3);
            var records = new List<CreateLogRecord>() { new CreateLogRecord { EventCode="TEST", ProjectId="TEST", Timestamp=oldDate } };

            await Assert.ThrowsExceptionAsync<BadLogRecordDateTimeException>(async () => await _service!.WriteLogBatch(records));
        }

        [TestMethod]
        public async Task WriteLogBatch_AllOk_DaoCalled()
        {
            var records = new List<CreateLogRecord> { new CreateLogRecord { EventCode = "TEST", ProjectId = "TEST", Timestamp = DateTime.UtcNow } };
            var logs = new List<Log> { new Log() { Data=JsonDocument.Parse("{}"), EventCode = "TEST", ProjectId = "TEST", Level=LogLevel.Information } };
            _mapper!.Map<List<CreateLogRecord>, List<Log>>(records).Returns(logs);

            await _service!.WriteLogBatch(records);

            await _logDao!.Received().InsertLogs(logs);
        }


        [TestMethod]
        public async Task WriteLog_AllOk_DaoCalled()
        {
            var record = new CreateLogRecord { EventCode = "TEST", ProjectId = "TEST", Timestamp = DateTime.UtcNow };
            var records = new List<CreateLogRecord> { record };
            var logs = new List<Log> { new Log() { Data =JsonDocument.Parse("{}"), EventCode = "TEST", ProjectId = "TEST", Level = LogLevel.Information } };
            _mapper!.Map<List<CreateLogRecord>, List<Log>>(records).ReturnsForAnyArgs(logs);

            await _service!.WriteLog(record);

            await _logDao!.Received().InsertLogs(logs);
        }
    }
}
