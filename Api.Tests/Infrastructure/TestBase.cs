using Api.Infrastructure.Persistence;
using Api.Tests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests.Infrastructure;

public abstract class TestBase
{
    private IServiceScope _scope = null!;
    protected ISender Mediator => _scope.ServiceProvider.GetRequiredService<ISender>();
    protected ApplicationDbContext DbContext =>
        _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    [SetUp]
    public async Task SetUp()
    {
        await TestInitializer.ResetDatabaseAsync();
        _scope = TestInitializer.Factory.Services.CreateScope();
    }

    [TearDown]
    public void TearDown()
    {
        _scope?.Dispose();
    }
}
