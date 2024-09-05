using Bookify.Application.Abstractions.Data;
using Bookify.Infrastructure;
using Bookify.Infrastructure.Authentication;
using Bookify.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Bookify.Application.IntegrationTests.Infrastructure;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest").WithDatabase("bookify").WithUsername("postgres").WithPassword("postgres").Build();

    private readonly KeycloakContainer _keycloakContainer = new KeycloakBuilder()
        .WithResourceMapping(new FileInfo(".files/bookify-realm-export.json"), new FileInfo("opt/keycloak/data/import/realm.json"))
        .WithCommand("--import-realm").Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder().WithImage("redis:latest").Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _keycloakContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _keycloakContainer.StopAsync();
        await _redisContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(serviceCollection =>
        {
            serviceCollection.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            serviceCollection.AddDbContext<ApplicationDbContext>(optionsBuilder =>
                optionsBuilder.UseNpgsql(_dbContainer.GetConnectionString()).UseSnakeCaseNamingConvention());
            serviceCollection.RemoveAll(typeof(ISqlConnectionFactory));
            serviceCollection.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(_dbContainer.GetConnectionString()));
            serviceCollection.Configure<RedisCacheOptions>(redisCacheOptions => redisCacheOptions.Configuration = _redisContainer.GetConnectionString());
            var keycloakAddress = _keycloakContainer.GetBaseAddress();
            serviceCollection.Configure<KeycloakOptions>(o =>
            {
                o.AdminUrl = $"{keycloakAddress}admin/realms/bookify/";
                o.TokenUrl = $"{keycloakAddress}realms/bookify/protocol/openid-connect/token";
            });
        });
    }
}