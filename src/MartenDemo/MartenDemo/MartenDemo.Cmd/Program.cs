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
});

await using var session = store.LightweightSession();

var stream = session.Events.StartStream(boxId, new BoxCreated(boxId));

for (int i = 0; i < 10; i++)
{
    session.Events.Append(boxId, new BeerAdded(boxId, $"AwesomeBeer {i}"));
}

await session.SaveChangesAsync();

Console.WriteLine("Done.");

public record BoxCreated(Guid BoxId);
public record BeerAdded(Guid BoxId, string BeerName);

public record BoxAggregate(Guid BoxId, string[] Beers);