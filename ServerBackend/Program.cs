using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServerBackend.Data;
using ServerBackend.Helpers;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

if (config["REAL_DATABASE"] == null || config["REAL_DATABASE"] == "False")
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

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "MeerKat",
            ValidAudience = "mircats",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MeerKat è un software di gestione aziendale"))
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.Run();