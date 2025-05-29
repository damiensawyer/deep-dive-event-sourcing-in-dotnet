using Marten;
using Marten.Events.Projections;

var connectionString = """
                       User ID=postgres;
                       Password=Marten123;
                       Host=localhost;
                       Port=5432;
                       Database=marten-demo;
                       Pooling=true;
                       """;

var boxId = Guid.NewGuid();

// Demo here
var store = DocumentStore.For(options =>
{
    options.Connection(connectionString);

    options.Schema.For<BoxAggregate>().Identity(p => p.BoxId);
    
    options.Projections.Add<BeerCountProjection>(ProjectionLifecycle.Inline);
});

await using var session = store.IdentitySession();

var stream = session.Events.StartStream<BoxAggregate>(boxId, new BoxCreated(boxId));

for (int i = 0; i < 10; i++)
{
    session.Events.Append(boxId, new BeerAdded(boxId, $"AwesomeBeer {i}"));
}

await session.SaveChangesAsync();

await using var session2 = store.LightweightSession();

var aggregate = await session2.Events.AggregateStreamAsync<BoxAggregate>(boxId, version: 6);

var beerCount = await session2.LoadAsync<BeerCount>(boxId);

Console.WriteLine("Done.");

public record BoxCreated(Guid BoxId);
public record BeerAdded(Guid BoxId, string BeerName);

public record BoxAggregate(Guid BoxId, string[] Beers)
{
    public static BoxAggregate Create(BoxCreated created)
        => new BoxAggregate(created.BoxId, []);

    public static BoxAggregate Apply(BeerAdded beerAdded, BoxAggregate previous)
        => previous with { Beers = previous.Beers.Append(beerAdded.BeerName).ToArray() };
}

public class BeerCountProjection : EventProjection
{
    public BeerCountProjection()
    {
        Project<BoxCreated>((e, operations) =>
        {
            operations.Store(new BeerCount { Id = e.BoxId });
        });
        
        ProjectAsync<BeerAdded>(async (e, operations) =>
        {
            var beerCount = await operations.LoadAsync<BeerCount>(e.BoxId);
            beerCount.NumberOfBeers++;
            operations.Store(beerCount);
        });
    }
}

public class BeerCount
{
    public Guid Id { get; set; }
    public int NumberOfBeers { get; set; }
}