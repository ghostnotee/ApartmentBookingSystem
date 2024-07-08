namespace Bookify.Domain.Entities.Apartments;

public record Money(decimal Amount, Currency Currency)
{
    public static Money operator +(Money first, Money second)
    {
        if (first.Currency != second.Currency) throw new InvalidOperationException("Cuurencies have to be equal");

        return first with { Amount = first.Amount + second.Amount };
    }

    public static Money Zero() => new(0, Currency.None);
}