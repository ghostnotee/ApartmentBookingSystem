using Microsoft.Extensions.Options;
using Quartz;

namespace Bookify.Infrastructure.Outbox;

internal class ProcessOutboxMessagesJobSetup : IConfigureOptions<QuartzOptions>
{
    private readonly OutboxOptions _outboxOptions;

    public ProcessOutboxMessagesJobSetup(IOptions<OutboxOptions> outboxOptions)
    {
        _outboxOptions = outboxOptions.Value;
    }

    public void Configure(QuartzOptions options)
    {
        const string jobName = nameof(ProcessOutboxMessagesJob);
        options
            .AddJob<ProcessOutboxMessagesJob>(builder => builder.WithIdentity(jobName))
            .AddTrigger(builder => builder.ForJob(jobName)
                .WithSimpleSchedule(scheduleBuilder =>
                    scheduleBuilder.WithIntervalInSeconds(_outboxOptions.IntervalInSeconds).RepeatForever()));
    }
}