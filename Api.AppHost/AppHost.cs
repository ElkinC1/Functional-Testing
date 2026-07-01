var builder = DistributedApplication.CreateBuilder(args);

var db = builder
    .AddSqlServer("bdserver")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("db");

builder.AddProject<Projects.Api>("api").WithExternalHttpEndpoints().WithReference(db).WaitFor(db);
builder.Build().Run();
