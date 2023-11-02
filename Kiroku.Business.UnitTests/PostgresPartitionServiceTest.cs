using Kiroku.Business.Services;
using Kiroku.Business.Services.Impl;
using Kiroku.Persistence.Dao;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Business.UnitTests
{
    [TestClass]
    public class PostgresPartitionServiceTest
    {
        private IPartitionListAnalyzeStrategy? _analyzeStrategy;
        private IPartitionTableNamingStrategy? _namingStrategy;
        private ILogger<PostgresPartitionService>? _logger;
        private IPostgresPartitionDao? _partitionDao;

        private PostgresPartitionService? _service;

        [TestInitialize]
        public void TestInitialize()
        {
            _analyzeStrategy = Substitute.For<IPartitionListAnalyzeStrategy>();
            _namingStrategy = Substitute.For<IPartitionTableNamingStrategy>();
            _logger = Substitute.For<ILogger<PostgresPartitionService>>();
            _partitionDao = Substitute.For<IPostgresPartitionDao>();
            _service = new PostgresPartitionService(_analyzeStrategy, _namingStrategy, _logger, _partitionDao);
        }

        [TestMethod]
        public async Task CreateNextPartition_NoPartitions_Created()
        {
            var names = Array.Empty<string>();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var tomorrow = today.AddDays(1);
            var partitionName = "2023-10-20";
            _partitionDao!.GetLogsTablePartitions().Returns(names);
            _analyzeStrategy!.GetLastPartitionsDate(Array.Empty<DateOnly>()).ReturnsForAnyArgs(today);
            _analyzeStrategy.CheckCreatingNewPartitionIsRequired(today).Returns(true);
            _namingStrategy!.MakePartitionName(today).ReturnsForAnyArgs(partitionName);

            var result = await _service!.CreateNextPartition();

            await _partitionDao.Received().CreateTablePartition(partitionName, 
                today.ToDateTime(new TimeOnly(), DateTimeKind.Utc), 
                tomorrow.ToDateTime(new TimeOnly(), DateTimeKind.Utc));
            Assert.AreEqual(CreateNextPartitionResult.Created, result);
        }

        [TestMethod]
        public async Task CreateNextPartition_HavePartition2Days_NoNeed()
        {
            var partitionName = "2023-10-22";
            var names = new string[] { partitionName };
            var today = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2);
            var tomorrow = today.AddDays(1);
            _partitionDao!.GetLogsTablePartitions().Returns(names);
            _analyzeStrategy!.GetLastPartitionsDate(Array.Empty<DateOnly>()).ReturnsForAnyArgs(today);
            _analyzeStrategy.CheckCreatingNewPartitionIsRequired(today).Returns(false);
            _namingStrategy!.MakePartitionName(today).ReturnsForAnyArgs(partitionName);

            var result = await _service!.CreateNextPartition();

            await _partitionDao.DidNotReceive().CreateTablePartition(partitionName,
                today.ToDateTime(new TimeOnly(), DateTimeKind.Utc),
                tomorrow.ToDateTime(new TimeOnly(), DateTimeKind.Utc));
            Assert.AreEqual(CreateNextPartitionResult.NoNeed, result);
        }

        [TestMethod]
        public async Task DeleteOldPartitions_3Old1ActualPartitions_3TimesDeleted()
        {
            var oldDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-8);
            var actualDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-4);
            var oldDateString = oldDate.ToString("dd-M-yyyy", CultureInfo.InvariantCulture);
            var actualDateString = actualDate.ToString("dd-M-yyyy", CultureInfo.InvariantCulture);
            _partitionDao!.GetLogsTablePartitions().Returns(new string[] { actualDateString, oldDateString, oldDateString, oldDateString });
            _namingStrategy!.ParsePartitionName(oldDateString).Returns(oldDate);
            _namingStrategy.ParsePartitionName(actualDateString).Returns(actualDate);
            _namingStrategy.MakePartitionName(oldDate).Returns(oldDateString);
            _namingStrategy.MakePartitionName(actualDate).Returns(actualDateString);

            await _service!.DeleteOldPartitions();

            await _partitionDao.Received(3).DeletePartition(oldDateString);
            await _partitionDao.DidNotReceive().DeletePartition(actualDateString);
        }
    }
}
