using System.Data;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Domain.Abstractions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace Bookify.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxMessagesJob : IJob
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;
    private readonly OutboxOptions _outboxOptions;
    private readonly IPublisher _publisher;

    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public ProcessOutboxMessagesJob(
        ISqlConnectionFactory sqlConnectionFactory,
        IPublisher publisher,
        IDateTimeProvider dateTimeProvider,
        IOptions<OutboxOptions> outboxOptions,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _publisher = publisher;
        _dateTimeProvider = dateTimeProvider;
        _outboxOptions = outboxOptions.Value;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Beginning to process outbox messages");

        using var connection = _sqlConnectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        var outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

        foreach (var outboxMessageResponse in outboxMessages)
        {
            Exception? exception = null;
            try
            {
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessageResponse.Content, JsonSerializerSettings)!;
                await _publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while processing outbox message {MessageId}", outboxMessageResponse.Id);
                exception = e;
            }

            await UpdateOutboxMessageAsync(connection, transaction, outboxMessageResponse, exception);
        }

        transaction.Commit();
        _logger.LogInformation("Completed processing outbox messages");
    }

    private async Task UpdateOutboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxMessageResponse outboxMessageResponse,
        Exception? exception)
    {
        const string sql = @"
            UPDATE outbox_messages
            SET processed_on_utc = @ProcessedOnUtc,
                error = @Error
            WHERE id = @Id";

        await connection.ExecuteAsync(sql, new
            {
                outboxMessageResponse.Id,
                ProcessedOnUtc = _dateTimeProvider.UtcNow,
                Error = exception?.ToString()
            },
            transaction);
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(IDbConnection connection, IDbTransaction transaction)
    {
        var sql = $"""
                   SELECT id, content
                   FROM outbox_messages
                   WHERE processed_on_utc IS NULL
                   ORDER BY occurred_on_utc
                   LIMIT {_outboxOptions.BatchSize}
                   FOR UPDATE 
                   """;
        var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(sql, transaction: transaction);
        return outboxMessages.ToList();
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);
}