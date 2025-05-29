using BeerSender.Domain.Boxes;

namespace BeerSender.Domain.Tests.Boxes;

public abstract class BoxTest<TCommand> : CommandHandlerTest<TCommand>
{
    protected Guid Box_Id => _aggregateId;
    
    // Events
    protected BoxCreated Box_created_with_capacity(int capacity)
    {
        return new BoxCreated(new BoxCapacity(capacity));
    }

    protected BeerBottleAdded Beer_bottle_added(BeerBottle bottle)
    {
        return new BeerBottleAdded(bottle);
    }

    protected FailedToAddBeerBottle Box_was_full()
    {
        return new FailedToAddBeerBottle(FailedToAddBeerBottle.FailReason.BoxWasFull);
    }

    protected ShippingLabelAdded Shipping_label_added_with_carrier_and_code(Carrier carrier, string trackingCode)
    {
        return new ShippingLabelAdded(new ShippingLabel(carrier, trackingCode));
    }

    protected FailedToAddShippingLabel Tracking_code_was_invalid()
    {
        return new FailedToAddShippingLabel(FailedToAddShippingLabel.FailReason.TrackingCodeInvalid);
    }

    protected BoxClosed Box_was_closed()
    {
        return new BoxClosed();
    }

    protected FailedToCloseBox Box_was_empty()
    {
        return new FailedToCloseBox(FailedToCloseBox.FailReason.BoxWasEmpty);
    }

    protected BoxSent Box_was_sent()
    {
        return new BoxSent();
    }

    protected FailedToSendBox Box_was_not_closed()
    {
        return new FailedToSendBox(FailedToSendBox.FailReason.BoxWasNotClosed);
    }

    protected FailedToSendBox Box_has_no_label()
    {
        return new FailedToSendBox(FailedToSendBox.FailReason.BoxHadNoLabel);
    }
    
    // Test data
    protected BeerBottle gouden_carolus = new(
        "Gouden Carolus",
        "Quadrupel Whisky Infused",
        12.7,
        BeerType.Quadruple
    );
    
    protected BeerBottle carte_blanche = new(
        "Wolf",
        "Carte Blanche",
        8.5,
        BeerType.Triple
    );
}