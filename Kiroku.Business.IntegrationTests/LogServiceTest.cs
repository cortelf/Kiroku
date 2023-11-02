using AutoMapper;
using Kiroku.Business.Impl;
using Kiroku.Business.MappingProfiles;
using Kiroku.Business.Models;
using Kiroku.Business.Services.Impl;
using Kiroku.Persistence;
using Kiroku.Persistence.Dao;
using Kiroku.Persistence.Dao.Impl;
using Kiroku.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using NSubstitute;
using Testcontainers.PostgreSql;

namespace Kiroku.Business.IntegrationTests
{
    [TestClass]
    public class LogServiceTest
    {
        private PostgresPartitionService? _partitionService;
        private LogService? _logService;
        private ILogDao? _logDao;
        private IMapper? _mapper;


        private static PostgreSqlContainer _postgres = new PostgreSqlBuilder()
           .WithImage("postgres:16-alpine")
           .Build();

        private static KirokuDatabaseContext? _kirokuDatabaseContext;

        private static async Task PrepareContext()
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_postgres.GetConnectionString());
            dataSourceBuilder.MapEnum<LogLevel>();
            var dataSource = dataSourceBuilder.Build();
            var opts = new DbContextOptionsBuilder<KirokuDatabaseContext>()
                .UseNpgsql(dataSource, opt => opt.MigrationsAssembly(typeof(KirokuDatabaseContext).Assembly.FullName));
            _kirokuDatabaseContext = new KirokuDatabaseContext(opts.Options);
            await _kirokuDatabaseContext.Database.MigrateAsync();
        }

        [ClassInitialize]
        public static async Task Initialize(TestContext testContext)
        {
            await _postgres.StartAsync().ConfigureAwait(false);
            await PrepareContext();
            await PrepareContext(); // https://github.com/npgsql/efcore.pg/issues/292
        }


        [TestInitialize]
        public void TestInitialize()
        {
            var analyzeStrategy = new PartitionListAnalyzeStrategy();
            var namingStrategy = new PartitionTableNamingStrategy();
            var logger = Substitute.For<ILogger<PostgresPartitionService>>();
            var partitionDao = new PostgresPartitionDao(_kirokuDatabaseContext!);
            _partitionService = new PostgresPartitionService(analyzeStrategy, namingStrategy, logger, partitionDao);

            _logDao = new LogDao(_kirokuDatabaseContext!);
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<LogRecordProfile>()));
            _logService = new LogService(_logDao, _mapper);
        }

        [TestMethod]
        public async Task WriteLogBatch_0_WriteWithoutPartition_NoPartitionException()
        {
            var records = new List<CreateLogRecord> { new CreateLogRecord { EventCode = "TEST", ProjectId = "TEST", Timestamp = DateTime.UtcNow } };

            await Assert.ThrowsExceptionAsync<NoPartitionException>(
                async () => await _logService!.WriteLogBatch(records));
        }

        [TestMethod]
        public async Task WriteLogBatch_AllOk_NoErrors()
        {
            var records = new List<CreateLogRecord> { new CreateLogRecord { EventCode = "TEST", ProjectId = "TEST", Timestamp = DateTime.UtcNow } };

            await _partitionService!.CreateNextPartition();
            await _partitionService!.CreateNextPartition();
            await _logService!.WriteLogBatch(records);
        }
    }


}
