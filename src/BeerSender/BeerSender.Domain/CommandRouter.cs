using Marten;

namespace BeerSender.Domain;

public class CommandRouter(
    IServiceProvider serviceProvider,
    IDocumentStore store)
{
    public async Task HandleCommand(ICommand command)
    {
        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
        var handler = serviceProvider.GetService(handlerType);
        var methodInfo = handlerType.GetMethod("Handle");

        await using var session = store.IdentitySession();
        
        var handle = (Task)methodInfo?.Invoke(handler, [session, command]);
        await handle;

        await session.SaveChangesAsync();
    }
}