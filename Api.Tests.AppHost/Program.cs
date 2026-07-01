var builder = DistributedApplication.CreateBuilder(args);

builder.AddSqlServer("bdserver").AddDatabase("db");

builder.Build().Run();
