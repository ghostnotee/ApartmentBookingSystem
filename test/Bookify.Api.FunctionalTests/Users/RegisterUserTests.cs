using System.Net;
using System.Net.Http.Json;
using Bookify.Api.Controllers.Users;
using Bookify.Api.FunctionalTests.Infrastructure;
using FluentAssertions;

namespace Bookify.Api.FunctionalTests.Users;

public class RegisterUserTests : BaseFunctionalTest
{
    public RegisterUserTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
    }

    [Theory]
    [InlineData("", "first", "last", "12345")]
    [InlineData("dogan.com", "first", "last", "12345")]
    [InlineData("@dogan.com", "first", "last", "12345")]
    [InlineData("nihat@", "first", "last", "12345")]
    [InlineData("nihat@dogan.com", "", "last", "12345")]
    [InlineData("nihat@dogan.com", "first", "", "12345")]
    [InlineData("nihat@dogan.com", "first", "last", "")]
    [InlineData("nihat@dogan.com", "first", "last", "1")]
    [InlineData("nihat@dogan.com", "first", "last", "12")]
    [InlineData("nihat@dogan.com", "first", "last", "123")]
    [InlineData("nihat@dogan.com", "first", "last", "1234")]
    public async Task Register_ShouldReturnBadRequest_WhenRequestIsInvalid(
        string email,
        string firstName,
        string lastName,
        string password)
    {
        // Arrange
        var request = new RegisterUserRequest(email, firstName, lastName, password);

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterUserRequest("mahmut@tuncer.com", "first", "last", "12345");

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}