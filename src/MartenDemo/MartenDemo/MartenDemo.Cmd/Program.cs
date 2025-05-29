using Marten;

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
});

await using var session = store.LightweightSession();

var stream = session.Events.StartStream<BoxAggregate>(boxId, new BoxCreated(boxId));

for (int i = 0; i < 10; i++)
{
    session.Events.Append(boxId, new BeerAdded(boxId, $"AwesomeBeer {i}"));
}

await session.SaveChangesAsync();

await using var session2 = store.LightweightSession();

var aggregate = await session2.Events.AggregateStreamAsync<BoxAggregate>(boxId, version: 6);

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