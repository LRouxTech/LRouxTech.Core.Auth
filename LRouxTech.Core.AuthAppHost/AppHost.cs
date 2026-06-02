var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres-server");
var testDb = postgres.AddDatabase("testdb");

// Reference the database in your API project
builder.AddProject<Projects.LRouxTech_Core_AuthSample>("api-service")
    .WithReference(testDb)
    .WaitFor(testDb);

builder.Build().Run();