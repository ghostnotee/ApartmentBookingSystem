namespace Bookify.Application.Abstractions.Clock;

public interface IDatetimeProvider
{
    DateTime UtcNow { get; }
}