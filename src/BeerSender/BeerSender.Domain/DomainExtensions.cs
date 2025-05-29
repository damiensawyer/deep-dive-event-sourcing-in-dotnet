using BeerSender.Domain.Boxes.Commands;
using BeerSender.Domain.Projections;
using Marten;
using Marten.Events.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace BeerSender.Domain;

public static class DomainExtensions
{
    public static void RegisterDomain(this IServiceCollection services)
    {
        services.AddScoped<CommandRouter>();
        
        services.AddTransient<ICommandHandler<CreateBox>, CreateBoxHandler>();
        services.AddTransient<ICommandHandler<AddShippingLabel>, AddShippingLabelHandler>();
        services.AddTransient<ICommandHandler<AddBeerBottle>, AddBeerBottleHandler>();
        services.AddTransient<ICommandHandler<CloseBox>, CloseBoxHandler>();
        services.AddTransient<ICommandHandler<SendBox>, SendBoxHandler>();
    }
    
    public static void ApplyDomainConfig(this StoreOptions options)
    {
        options.UseSystemTextJsonForSerialization();
        
        options.Schema.For<UnsentBox>().Identity(u => u.BoxId);
        options.Schema.For<OpenBox>().Identity(o => o.BoxId);
    }
    
    public static void AddProjections(this StoreOptions options)
    {
        options.Projections.Add<UnsentBoxProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<OpenBoxProjection>(ProjectionLifecycle.Async);
    }
}