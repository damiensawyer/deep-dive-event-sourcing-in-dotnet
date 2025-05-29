using Marten;

namespace BeerSender.Domain.Boxes.Commands;

public record CloseBox
(
    Guid BoxId
) : ICommand;

public class CloseBoxHandler
    : ICommandHandler<CloseBox>
{
    public async Task Handle(IDocumentSession session, CloseBox command)
    {
        var box = await session.Events.AggregateStreamAsync<Box>(command.BoxId);

        if (box.BeerBottles.Any())
        {
            session.Events.Append(command.BoxId, new BoxClosed());
        }
        else
        {
            session.Events.Append(command.BoxId, new FailedToCloseBox(FailedToCloseBox.FailReason.BoxWasEmpty));
        }
    }
}