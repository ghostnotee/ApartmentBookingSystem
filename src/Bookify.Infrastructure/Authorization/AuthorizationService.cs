using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;

internal sealed class AuthorizationService
{
    private readonly ApplicationDbContext _dbContext;

    public AuthorizationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
    {
        var roles = await _dbContext.Set<User>().Where(x => x.IdentityId == identityId).Select(x => new UserRolesResponse
        {
            Id = x.Id,
            Roles = x.Roles.ToList()
        }).FirstAsync();
        return roles;
    }
}