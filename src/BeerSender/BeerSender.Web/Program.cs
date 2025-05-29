using BeerSender.Domain;
using BeerSender.EventStore;
using BeerSender.Web.EventPublishing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.RegisterDomain();
builder.Services.RegisterEventStore();
builder.Services.AddTransient<INotificationService, NotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/openapi/v1.json", "BeerSender API"));
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();
app.MapHub<EventHub>("event-hub");

app.Run();