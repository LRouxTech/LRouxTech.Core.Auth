using LRouxTech.Core.Auth.Api.Authorization;
using LRouxTech.Core.Auth.Api.Endpoints;
using LRouxTech.Core.Auth.Api.Extensions;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Mail;
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
            x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "User");
            x.MigrationsAssembly("LRouxTech.Core.Auth");
        });
    });
}

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

builder.Services.AddAuthModule();
builder.Services.AddCustomPermissions<LocalPermissions>();
builder.Services.AddEmailInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var contextFactory = services.GetRequiredService<IDbContextFactory<UserContext>>();
        using var context = await contextFactory.CreateDbContextAsync();
            
        await context.Database.MigrateAsync(); 
        
        await RuntimeDataSeeder.SeedPermissionsAsync<LocalPermissions>(context);
        
        await RuntimeDataSeeder.SeedRolesAsync<LocalRoles>(context);

        await RuntimeDataSeeder.SeedAdminUserAsync(context);

        await RuntimeDataSeeder.SyncRolePermissionsAsync(context, LocalRoles.InventoryAuditor, [
            LocalPermissions.Inventory.AdjustStock,
            LocalPermissions.Inventory.ViewStock
        ]);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database initialization or data seeding.");
    }
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