using BeerSender.Domain;
using BeerSender.Web.EventPublishing;
using Marten;
using Marten.Events.Daemon.Resiliency;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.RegisterDomain();
builder.Services.AddTransient<INotificationService, NotificationService>();

builder.Services.AddMarten(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("Marten");
    opt.Connection(connectionString!);
    
    opt.ApplyDomainConfig();
    opt.AddProjections();
})
.AddAsyncDaemon(DaemonMode.Solo);

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