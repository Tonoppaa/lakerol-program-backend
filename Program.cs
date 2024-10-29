using backend_Lakerol.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

/* CORS (Cross-Origin Resource Sharing) on verkkoturvaprotokolla, joka mahdollistaa resurssien jakamisen eri alkuperien (origins) välillä. 
 * Alkuperä (origin) määritellään URL perusteella, joka koostuu seuraavista osista: protokolla (esim. http tai https), palvelimen osoite 
 * (esim. www.esimerkki.com) ja portti (esim. :3000). */

/* Tämä rivi lisää CORS-palvelun sovelluksen palvelinrekisteriin. builder on instanssi, joka luodaan WebApplication.CreateBuilder(args) 
 * -metodilla, ja se tarjoaa pääsyn palveluiden rekisteröimiseen. */
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Entity Framework ja MySQL -tietokantayhteys
builder.Services.AddDbContext<YourDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("lakerolCon"),
    new MySqlServerVersion(new Version(8, 0, 21))));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
