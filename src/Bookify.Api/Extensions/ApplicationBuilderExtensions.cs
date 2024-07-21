using Bookify.Api.Middleware;
using Bookify.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder application)
    {
        using var scope = application.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }

    public static void UseCustomExceptionHandler(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}