using Kiroku.Business.Impl;
using Kiroku.Business.Services.Impl;
using Kiroku.Persistence;
using Kiroku.Persistence.Dao;
using Kiroku.Persistence.Dao.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace Kiroku.Business.IntegrationTests
{
    [TestClass]
    public class PostgresPartitionServiceTest
    {
        private IPartitionListAnalyzeStrategy _analyzeStrategy;
        private IPartitionTableNamingStrategy _namingStrategy;
        private ILogger<PostgresPartitionService> _logger;
        private IPostgresPartitionDao _partitionDao;

        private PostgresPartitionService _service;

        private static PostgreSqlContainer _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .Build();

        private static KirokuDatabaseContext _kirokuDatabaseContext;

        [TestInitialize]
        public void TestInitialize()
        {
            _analyzeStrategy = new PartitionListAnalyzeStrategy();
            _namingStrategy = new PartitionTableNamingStrategy();
            _logger = Substitute.For<ILogger<PostgresPartitionService>>();
            _partitionDao = new PostgresPartitionDao(_kirokuDatabaseContext);
            _service = new PostgresPartitionService(_analyzeStrategy, _namingStrategy, _logger, _partitionDao);


        }

        [ClassInitialize]
        public static async Task Initialize(TestContext testContext)
        {
            await _postgres.StartAsync().ConfigureAwait(false);
            var opts = new DbContextOptionsBuilder<KirokuDatabaseContext>().UseNpgsql(_postgres.GetConnectionString());
            _kirokuDatabaseContext = new KirokuDatabaseContext(opts.Options);
            await _kirokuDatabaseContext.Database.MigrateAsync();
        }

        [ClassCleanup]
        public static Task Cleanup()
        {
            return _postgres.DisposeAsync().AsTask();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            var partitions = await _partitionDao.GetLogsTablePartitions();
            foreach (var partition in partitions)
            {
                await _partitionDao.DeletePartition(partition);
            }
        }

        [TestMethod]
        public async Task CreateNextPartition_NoPartitions_CreatedOne()
        {
            await _service.CreateNextPartition();
            var partitions = await _partitionDao.GetLogsTablePartitions();

            Assert.AreEqual(1, partitions.Count() );
        }

        [TestMethod]
        public async Task CreateNextPartition_NoPartitionsCalledCreate3Times_CreatedTwo()
        {
            var firstAnswer = await _service.CreateNextPartition();
            var secondAnswer = await _service.CreateNextPartition();
            var thirdAnswer = await _service.CreateNextPartition();
            var partitions = await _partitionDao.GetLogsTablePartitions();

            Assert.AreEqual(2, partitions.Count());
            Assert.AreEqual(firstAnswer, Services.CreateNextPartitionResult.Created);
            Assert.AreEqual(secondAnswer, Services.CreateNextPartitionResult.Created);
            Assert.AreEqual(thirdAnswer, Services.CreateNextPartitionResult.NoNeed);
        }

        [TestMethod]
        public async Task CreateAndDeletePartition_1Partition_NoErrors()
        {
            var partitionDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-8);
            var partitionName = _namingStrategy.MakePartitionName(partitionDate);

            await _partitionDao.CreateTablePartition(partitionName, 
                partitionDate.AddDays(-1).ToDateTime(new TimeOnly(), DateTimeKind.Utc),
                partitionDate.ToDateTime(new TimeOnly(), DateTimeKind.Utc));
            await _service.DeleteOldPartitions();
            var partitions = await _partitionDao.GetLogsTablePartitions();


            Assert.AreEqual(0, partitions.Count());
        }
    }
}
