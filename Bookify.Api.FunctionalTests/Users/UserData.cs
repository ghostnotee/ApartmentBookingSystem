using Bookify.Api.Controllers.Users;

namespace Bookify.Api.FunctionalTests.Users;

internal static class UserData
{
    public static RegisterUserRequest RegisterTestUserRequest = new("nihat@dogan.com", "test", "test", "12345");
}