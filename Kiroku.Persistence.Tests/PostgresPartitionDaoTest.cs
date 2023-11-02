using Kiroku.Persistence.Dao.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace Kiroku.Persistence.Tests
{
    [TestClass]
    public class PostgresPartitionDaoTest
    {
        private static PostgreSqlContainer _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .Build();

        private static KirokuDatabaseContext? _kirokuDatabaseContext;
        private static PostgresPartitionDao? _dao; 

        [ClassInitialize]
        public static async Task Initialize(TestContext testContext)
        {
            await _postgres.StartAsync().ConfigureAwait(false);
            var opts = new DbContextOptionsBuilder<KirokuDatabaseContext>().UseNpgsql(_postgres.GetConnectionString());
            _kirokuDatabaseContext = new KirokuDatabaseContext(opts.Options);
            await _kirokuDatabaseContext.Database.MigrateAsync();
            _dao = new PostgresPartitionDao(_kirokuDatabaseContext);
        }

        [ClassCleanup]
        public static Task Cleanup()
        {
            return _postgres.DisposeAsync().AsTask();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            var partitions = await _dao!.GetLogsTablePartitions();
            foreach (var partition in partitions)
            {
                await _dao.DeletePartition(partition);
            }
        }

        [TestMethod]
        public async Task GetLogsTablePartitions_NoPartitions_AnyFalse()
        {
            var emptyPartitions = await _dao!.GetLogsTablePartitions();

            Assert.IsFalse(emptyPartitions.Any());
        }

        [TestMethod]
        public async Task GetLogsTablePartitions_OnePartition_SingleElementAndNameEquals()
        {
            var partitionName = "test_partition";

            await _dao!.CreateTablePartition(partitionName, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
            var partitions = await _dao.GetLogsTablePartitions();

            Assert.IsTrue(partitions.Count() == 1);
            Assert.AreEqual(partitionName, partitions.ElementAt(0));
        }

        [TestMethod]
        public async Task CreateTablePartition_AllOk_NoError()
        {
            var partitionName = "test_partition";

            await _dao!.CreateTablePartition(partitionName, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
        }

        [TestMethod]
        public async Task CreateTablePartition_DuplicateNamePartition_ThrowPostgresException()
        {
            var partitionName = "test_partition";

            await _dao!.CreateTablePartition(partitionName, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

            await Assert.ThrowsExceptionAsync<PostgresException>(() => 
                _dao.CreateTablePartition(partitionName, DateTime.UtcNow.AddDays(20), DateTime.UtcNow.AddDays(21)));
        }

        [TestMethod]
        public async Task CreateTablePartition_CrossingPartitionDates_ThrowPostgresException()
        {
            var partitionName = "test_partition";
            var secondPartitionName = "test_partition2";

            await _dao!.CreateTablePartition(partitionName, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

            await Assert.ThrowsExceptionAsync<PostgresException>(() =>
                _dao.CreateTablePartition(secondPartitionName, DateTime.UtcNow, DateTime.UtcNow.AddDays(2)));
        }

        [TestMethod]
        public async Task CreateTablePartition_DateToBeforeFrom_ThrowPostgresException()
        {
            var partitionName = "test_partition";

            await Assert.ThrowsExceptionAsync<PostgresException>(() =>
                _dao!.CreateTablePartition(partitionName, DateTime.UtcNow.AddDays(1), DateTime.UtcNow));
        }

        [TestMethod]
        public async Task DeletePartition_AllOk_NoError()
        {
            var partitionName = "test_partition";

            await _dao!.CreateTablePartition(partitionName, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

            await _dao.DeletePartition(partitionName);
        }

        [TestMethod]
        public async Task DeletePartition_PartitionNotFound_ThrowPostgresException()
        {
            var partitionName = "test_partition";

            await Assert.ThrowsExceptionAsync<PostgresException>(() =>
                _dao!.DeletePartition(partitionName));
        }
    }
}
