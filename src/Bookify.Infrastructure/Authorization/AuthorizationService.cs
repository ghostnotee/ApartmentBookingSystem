using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;

internal sealed class AuthorizationService(ApplicationDbContext dbContext)
{
    public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
    {
        var roles = await dbContext.Set<User>().Where(x => x.IdentityId == identityId).Select(x => new UserRolesResponse
        {
            Id = x.Id,
            Roles = x.Roles.ToList()
        }).FirstAsync();
        return roles;
    }

    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        var permissions = await dbContext.Set<User>().Where(user => user.IdentityId == identityId)
            .SelectMany(user => user.Roles.Select(role => role.Permissions)).FirstAsync();
        var permissionsSet = permissions.Select(permission => permission.Name).ToHashSet();
        return permissionsSet;
    }
}