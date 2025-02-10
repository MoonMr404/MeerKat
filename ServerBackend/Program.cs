using Microsoft.EntityFrameworkCore;
using ServerBackend.Data;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

if (config["REAL_DATABASE"] == null || config["REAL_DATABASE"] == "false")
{
    //Memory database per il testing
    builder.Services.AddDbContext<MeerkatContext>(options =>
        options.UseInMemoryDatabase("meerkat"));
}
else
{
    //Database SQL, controllare appsettings.Development.json per l'acesso
    var connectionString = builder.Configuration.GetConnectionString("Local");
    builder.Services.AddDbContext<MeerkatContext>(options =>
         options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))); 
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();