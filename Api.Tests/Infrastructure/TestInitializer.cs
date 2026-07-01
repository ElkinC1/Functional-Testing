extern alias TestAppHostAlias;

using Api.Infrastructure.Persistence;
using Api.Tests.Infrastructure;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
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
        var appBuilder =
            await DistributedApplicationTestingBuilder.CreateAsync<TestAppHostAlias::Projects.Api_Tests_AppHost>();
        AppHost = await appBuilder.BuildAsync();
        await AppHost.StartAsync();
        ConnectionString = await AppHost.GetConnectionStringAsync("db") ?? string.Empty;

        if (!string.IsNullOrEmpty(ConnectionString))
        {
            Environment.SetEnvironmentVariable("ConnectionStrings__db", ConnectionString);
        }

        Factory = new CustomWebApplicationFactory(ConnectionString);

        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var dbReady = false;
        var retries = 30;
        while (!dbReady && retries > 0)
        {
            try
            {
                await context.Database.EnsureCreatedAsync();
                dbReady = true;
            }
            catch (Exception)
            {
                await Task.Delay(1000);
                retries--;
                if (retries == 0)
                {
                    throw;
                }
            }
        }

        var respawnerReady = false;
        var respawnerRetries = 10;
        while (!respawnerReady && respawnerRetries > 0)
        {
            try
            {
                _respawner = await Respawner.CreateAsync(
                    ConnectionString,
                    new RespawnerOptions { DbAdapter = DbAdapter.SqlServer }
                );
                respawnerReady = true;
            }
            catch (Exception)
            {
                await Task.Delay(1000);
                respawnerRetries--;
                if (respawnerRetries == 0)
                {
                    throw;
                }
            }
        }
    }

    [OneTimeTearDown]
    public async Task RunAfterAllTests()
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__db", null);

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
            var retries = 5;
            while (retries > 0)
            {
                try
                {
                    await _respawner.ResetAsync(ConnectionString);
                    break;
                }
                catch (Exception ex) when (ex.Message.Contains("connection") || ex is System.Net.Sockets.SocketException || ex is Microsoft.Data.SqlClient.SqlException)
                {
                    retries--;
                    if (retries == 0) throw;
                    await Task.Delay(500);
                }
            }
        }
    }
}
