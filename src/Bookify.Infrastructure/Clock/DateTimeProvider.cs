using Bookify.Application.Abstractions.Clock;

namespace Bookify.Infrastructure.Clock;

internal sealed class DateTimeProvider: IDatetimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}