using System.Data;
using Dapper;

namespace BeerSender.Projections.Database.Repositories;

public class UnsentBoxRepository(ReadStoreConnection dbConnection)
{
    private IDbConnection Connection => dbConnection.GetConnection();
    private IDbTransaction Transaction => dbConnection.GetTransaction();

    public void CreateUnsentBox(Guid boxId)
    {
        const string insertCommand = """
                                     INSERT INTO [dbo].[UnsentBoxes]
                                     (BoxId, Status) VALUES (@BoxId, 'Preparing')
                                     """;

        Connection.Execute(
            insertCommand,
            new { BoxId = boxId},
            Transaction);
    }

    public void SetStatus(Guid boxId, string status)
    {
        const string updateCommand = """
                                     UPDATE [dbo].[UnsentBoxes]
                                     SET [Status] = @Status
                                     WHERE BoxId = @BoxId
                                     """;

        Connection.Execute(
            updateCommand,
            new { BoxId = boxId, Status = status },
            Transaction);
    }

    public void RemoveUnsentBox(Guid boxId)
    {
        const string deleteCommand = """
                                     DELETE FROM [dbo].[UnsentBoxes]
                                     WHERE BoxId = @BoxId
                                     """;

        Connection.Execute(
            deleteCommand,
            new { BoxId = boxId },
            Transaction);
    }
}