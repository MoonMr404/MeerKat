using Microsoft.EntityFrameworkCore;
using ServerBackend.Data;
using Shared.Entities;
using Shared.Utils;

var builder = WebApplication.CreateBuilder(args);

/*
 * ENVIROMENT LOGIC
 */
var useInMemoryDatabase = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

if (useInMemoryDatabase)
{
    //Memory database for testing
    builder.Services.AddDbContext<MeerkatDatabase>(options =>
        options.UseInMemoryDatabase("TestDb"));
}
else
{
    // Use real database for deployment
    // TODO: Choose a provider
    // var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    // builder.Services.AddDbContext<MeerkatDatabase>(options =>
    //     options.UseSqlServer(connectionString)); 
}

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<MeerkatDatabase>();
}

app.Run();