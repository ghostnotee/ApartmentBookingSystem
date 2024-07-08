namespace Bookify.Domain.Entities.Apartments;

public record Currency
{
    internal static readonly Currency None = new("");
    public static readonly Currency Try = new("TRY");
    public static readonly Currency Usd = new("USD");
    public static readonly Currency Eur = new("EUR");

    public static readonly IReadOnlyCollection<Currency> All = new[]
    {
        Try,
        Usd,
        Eur
    };

    private Currency(string code)
    {
        Code = code;
    }

    public string Code { get; init; }

    public static Currency FromCode(string code)
    {
        return All.FirstOrDefault(c => c.Code == code) ?? throw new ApplicationException("The currency code is invalid");
    }
}