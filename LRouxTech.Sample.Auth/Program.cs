using LRouxTech.Core.Auth.Api.Authorization;
using LRouxTech.Core.Auth.Api.Endpoints;
using LRouxTech.Core.Auth.Api.Extensions;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Sample.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddScoped<IUserDbContextFactory, UserDbContextFactory>();

if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("testdb")))
{
    var conString = builder.Configuration.GetConnectionString("testdb");
    
    builder.Services.AddDbContextFactory<UserContext, UserDbContextFactory>(options =>
    {
        options.UseNpgsql(conString, x =>
        {
            x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "user");
            x.MigrationsAssembly("LRouxTech.Core.Auth");
        });
    });
}

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

builder.Services.AddAuthModule();
builder.Services.AddCustomPermissions<LocalPermissions>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<UserContext>>();
        
    using var context = await contextFactory.CreateDbContextAsync();
        
    await context.Database.MigrateAsync(); 
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapRoleEndpoints();
app.MapPermissionEndpoints();
app.MapUserEndpoints();

app.Run();