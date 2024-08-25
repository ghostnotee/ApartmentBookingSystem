using Bookify.Application.Apartments.SearchApartments;
using Bookify.Application.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace Bookify.Application.IntegrationTests.Apartments;

public class SearchApartmentsTests : BaseIntegrationTest
{
    public SearchApartmentsTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task SearchApartments_ShouldReturnEmptyList_WhenDateRangeIsInvalid()
    {
        // Arrange
        var query = new SearchApartmentQuery(new DateOnly(2024,1,10), new DateOnly(2022,1,1));
        
        // Act
        var result = await Sender.Send(query);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty(); 
    }
}