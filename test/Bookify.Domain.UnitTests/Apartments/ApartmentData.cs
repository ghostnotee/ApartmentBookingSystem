using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;

namespace Bookify.Domain.UnitTests.Apartments;

internal static class ApartmentData
{
    public static Apartment Create(Money price, Money? cleaningFee = null)
    {
        return new Apartment(
            Guid.NewGuid(),
            new Name("Test Apartment"),
            new Description("Test Description"),
            new Address("Country", "State", "ZipCode", "City", "Street"),
            price,
            cleaningFee ?? Money.Zero(),
            []);
    }
}