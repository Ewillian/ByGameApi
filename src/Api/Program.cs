using System.Diagnostics.CodeAnalysis;

using ByGameApi.Domain.Abstractions;
using ByGameApi.Domain.Services;
using ByGameApi.Infrastructure.Abstractions;
using ByGameApi.Infrastructure.DataAccess;
using ByGameApi.Infrastructure.Factories;
using ByGameApi.Infrastructure.Options;
using ByGameApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("DatabaseOptions"));
builder.Services.AddScoped<IMySqlConnectionFactory, MySqlConnectionFactory>();
builder.Services.AddScoped<IDbCommandExecutor, DbCommandExecutor>();
builder.Services.AddScoped<IByRepository, ByRepository>();
builder.Services.AddScoped<IScoreService, ScoreService>();

var app = builder.Build();

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

#pragma warning disable S3903
namespace ByGameApi.Api
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        protected Program()
        {
        }
    }
}
#pragma warning restore S3903