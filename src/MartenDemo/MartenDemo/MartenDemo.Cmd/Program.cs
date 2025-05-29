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

Console.WriteLine("Done.");

public record BoxCreated(Guid BoxId);
public record BeerAdded(Guid BoxId, string BeerName);

public record BoxAggregate(Guid BoxId, string[] Beers);