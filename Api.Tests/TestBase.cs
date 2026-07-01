using Api.Infrastructure.Persistence;
using Aspire.Hosting.Testing;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;

namespace Api.Tests;

[SetUpFixture]
public class TestInitializer
{
    public static DistributedApplication? AppHost { get; private set; }
    public static string ConnectionString { get; private set; } = string.Empty;
    public static CustomWebApplicationFactory Factory { get; private set; } = null!;
    private static Respawner? _respawner;

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        AppHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Api_Tests_AppHost>();
        await AppHost.StartAsync();
        ConnectionString = await AppHost.GetConnectionStringAsync("db");

        Factory = new CustomWebApplicationFactory(ConnectionString);

        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();

        _respawner = await Respawner.CreateAsync(ConnectionString, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer
        });
    }

    [OneTimeTearDown]
    public async Task RunAfterAllTests()
    {
        if (Factory != null)
        {
            await Factory.DisposeAsync();
        }

        if (AppHost != null)
        {
            await AppHost.DisposeAsync();
        }
    }

    public static async Task ResetDatabaseAsync()
    {
        if (_respawner != null)
        {
            await _respawner.ResetAsync(ConnectionString);
        }
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public CustomWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:db", _connectionString }
            });
        });
    }
}

public abstract class TestBase
{
    private IServiceScope _scope = null!;
    protected ISender Mediator { get; private set; } = null!;
    protected ApplicationDbContext DbContext { get; private set; } = null!;

    [SetUp]
    public async Task SetUp()
    {
        await TestInitializer.ResetDatabaseAsync();
        _scope = TestInitializer.Factory.Services.CreateScope();
        Mediator = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    [TearDown]
    public void TearDown()
    {
        _scope.Dispose();
    }
}
