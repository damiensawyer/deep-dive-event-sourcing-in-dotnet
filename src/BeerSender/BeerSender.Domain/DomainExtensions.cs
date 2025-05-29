using BeerSender.Domain.Boxes;
using BeerSender.Domain.Boxes.Commands;
using BeerSender.Domain.JsonConfiguration;
using BeerSender.Domain.Projections;
using Marten;
using Marten.Events.Projections;
using Marten.Services.Json.Transformations;
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
        options.UseSystemTextJsonForSerialization(configure: opt =>
        {
            opt.AllowOutOfOrderMetadataProperties = true;
            opt.TypeInfoResolver = new CommandTypeResolver();
        });
        
        options.Schema.For<UnsentBox>().Identity(u => u.BoxId);
        options.Schema.For<OpenBox>().Identity(o => o.BoxId);
        options.Schema.For<BottleInBoxes>().Identity(o => o.BottleId);
        options.Schema.For<LoggedCommand>().Identity(o => o.CommandId);

        options.Events.Upcast<BoxCreatedUpcaster>();
    }
    
    public static void AddProjections(this StoreOptions options)
    {
        options.Projections.Add<UnsentBoxProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<OpenBoxProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<BottleInBoxesProjection>(ProjectionLifecycle.Async);
    }
}

public class BoxCreatedUpcaster
    : EventUpcaster<BoxCreated, BoxCreatedWithContainerType>
{
    protected override BoxCreatedWithContainerType Upcast(BoxCreated oldEvent)
    {
        return new BoxCreatedWithContainerType(
            oldEvent.Capacity,
            string.Empty,
            ContainerType.Bottle
        );
    }
}