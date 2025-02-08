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
    var paolo = new User("Paolo", "Bianchi", "Paolo@gmail.com", "pollofritto1234", new DateOnly(2004, 04, 03));
    var paoloteam = new Team("Paolo's Team", paolo);
    context.Users.Add(paolo);
    context.Teams.Add(paoloteam);
    context.SaveChanges();
    Console.WriteLine(context.Users.Include(user => user.ManagedTeams).ToArray()[0].ManagedTeams);
    //TESTING
}

app.Run();