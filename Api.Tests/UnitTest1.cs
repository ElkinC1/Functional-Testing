using Api.Tests.Infrastructure;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Shouldly;

namespace Api.Tests;

public class Tests
{
    [Test]
    public async Task TestConnectionString()
    {
        var appHost = TestInitializer.AppHost.ShouldNotBeNull();
        var connStr = await appHost.GetConnectionStringAsync("db");
        connStr.ShouldNotBeNullOrEmpty();
    }
}
