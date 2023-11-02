using Kiroku.PartitionWorker;
using Kiroku.Persistence;
using Kiroku.Business;
using Kiroku.PartitionWorker.Jobs;
using Quartz;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddPersistence(hostContext.Configuration);
        services.AddBusiness();

        services.AddQuartz(q =>
        {
            var createJobKey = new JobKey("CreatePartitionJob");
            q.AddJob<CreatePartitionJob>(opts => opts.WithIdentity(createJobKey));

            q.AddTrigger(opts => opts
                .ForJob(createJobKey)
                .WithIdentity("CreatePartitionJob-trigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever()
                )
            );

            var deleteJobKey = new JobKey("DeletePartitionJob");
            q.AddJob<DeletePartitionsJob>(opts => opts.WithIdentity(deleteJobKey));

            q.AddTrigger(opts => opts
                .ForJob(deleteJobKey)
                .WithIdentity("DeletePartitionJob-trigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever()
                )
            );
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    })
    .Build();

host.Run();
