using Dapper;

namespace BeerSender.QueryAPI.Database;

public class BoxQueryRepository(ReadStoreConnectionFactory dbFactory)
{
    public IEnumerable<OpenBox> GetAllOpen()
    {
        const string query = """
                             SELECT BoxId ,Capacity, NumberOfBottles
                             FROM dbo.OpenBoxes
                             """;

        using var connection = dbFactory.Create();

        return connection.Query<OpenBox>(query);
    }
    
    public IEnumerable<UnsentBox> GetAllUnsent()
    {
        const string query = """
                             SELECT BoxId, Status
                             FROM dbo.UnsentBoxes
                             """;

        using var connection = dbFactory.Create();

        return connection.Query<UnsentBox>(query);
    }
}

public class UnsentBox
{
    public Guid BoxId { get; set; }
    public string? Status { get; set; }
}

public class OpenBox
{
    public Guid BoxId { get; set; }
    public int Capacity { get; set; }
    public int NumberOfBottles { get; set; }
}