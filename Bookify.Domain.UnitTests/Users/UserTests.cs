using Bookify.Domain.Users;
using Bookify.Domain.Users.Events;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Users;

public class UserTests
{
    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        // Arrange is in own file 
        
        // Act
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        
        // Assert
        user.FirstName.Should().Be(UserData.FirstName);
        user.LastName.Should().Be(UserData.LastName);
        user.Email.Should().Be(UserData.Email);
    }

    [Fact]
    public void Create_Should_RaiseUserCreatedDomainEvent()
    {
        // Act
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        
        // Assert
        var domainEvent = user.GetDomainEvents().OfType<UserCreatedDomainEvent>().SingleOrDefault();
        domainEvent.UserId.Should().Be(user.Id);
    }
}