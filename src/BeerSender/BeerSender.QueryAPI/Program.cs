using BeerSender.Domain;
using BeerSender.EventStore;
using BeerSender.QueryAPI.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.RegisterDomain();
builder.Services.RegisterEventStore();
builder.Services.RegisterReadDatabase();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/openapi/v1.json", "BeerSender QueryAPI"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
