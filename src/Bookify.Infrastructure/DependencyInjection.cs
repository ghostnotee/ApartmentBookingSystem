using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Email;
using Bookify.Infrastructure.Clock;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDatetimeProvider, DateTimeProvider>(); // or AddSingleton
        services.AddTransient<IEmailService, IEmailService>();

        var connectionString = configuration.GetConnectionString("Database") ?? throw new ArgumentNullException(nameof(configuration));
        services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });
        
        return services;
    }
}