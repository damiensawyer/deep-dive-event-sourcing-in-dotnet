using BeerSender.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace BeerSender.EventStore;

public static class EventStoreExtensions
{
    public static void RegisterEventStore(this IServiceCollection services)
    {
        services.AddSingleton<EventStoreConnectionFactory>();
        services.AddScoped<IEventStore, EventStore>();
    }
}