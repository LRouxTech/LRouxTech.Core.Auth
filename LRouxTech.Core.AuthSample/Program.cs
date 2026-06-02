using LRouxTech.Core.Auth.Api.Endpoints;
using LRouxTech.Core.Auth.Api.Extensions;
using LRouxTech.Core.Auth.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUserDbContextFactory, UserDbContextFactory>();

if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("DefaultConnection")))
{
    var conString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContextFactory<UserContext, UserDbContextFactory>(options =>
    {
        options.UseNpgsql(conString,
            o => o
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    });
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddAuthModule();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapRoleEndpoints();
app.MapPermissionEndpoints();
app.MapUserEndpoints();

app.Run();