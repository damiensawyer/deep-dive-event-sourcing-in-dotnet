using Marten;
using Microsoft.AspNetCore.Http;

namespace BeerSender.Domain;

public class CommandRouter(
    IServiceProvider serviceProvider,
    IDocumentStore store,
    IHttpContextAccessor httpContextAccessor)
{
    public async Task HandleCommand(ICommand command)
    {
        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
        var handler = serviceProvider.GetService(handlerType);
        var methodInfo = handlerType.GetMethod("Handle");

        await using var session = store.IdentitySession();
        
        // If you have a command ID from the outside (message ID, request ID, etc.)
        // You should use that over here
        var commandId = Guid.NewGuid();

        StoreCommand(session, command, commandId);
        ConfigureSession(session, commandId);
        
        var handle = (Task)methodInfo?.Invoke(handler, [session, command]);
        await handle;

        await session.SaveChangesAsync();
    }

    private void ConfigureSession(IDocumentSession session, Guid commandId)
    {
        session.CausationId = commandId.ToString();
        // If the correlation flows in from another system, set it here.
        session.CorrelationId = commandId.ToString();
        // Add custom headers if desired
        session.SetHeader("TraceIdentifier", httpContextAccessor.HttpContext?.TraceIdentifier ?? string.Empty);
    }

    private void StoreCommand(IDocumentSession session, ICommand command, Guid commandId)
    {
        LoggedCommand loggedCommand = new (
            commandId,
            httpContextAccessor.HttpContext?.User.Identity?.Name,
            DateTime.UtcNow,
            command);
        
        session.Insert(loggedCommand);
    }
}

public record LoggedCommand(
    Guid CommandId,
    string? UserName,
    DateTime Timestamp,
    ICommand Command);