using BeerSender.Domain.Boxes.Commands;

namespace BeerSender.Domain.Tests.Boxes;

public class CreateBoxTest : BoxTest<CreateBox>
{
    protected override CommandHandler<CreateBox> Handler
        => new CreateBoxHandler(eventStore);
    
    [InlineData(0, 6)]
    [InlineData(5, 6)]
    [InlineData(6, 6)]
    [InlineData(7, 12)]
    [InlineData(11, 12)]
    [InlineData(12, 12)]
    [InlineData(13, 24)]
    [InlineData(23, 24)]
    [InlineData(24, 24)]
    [Theory]
    public void WhenCreatedWithValidCapacity_ShouldCreateBox(
        int desired_spots, int actual_spots)
    {
        Given(
        );
        When(
            Create_box_for_capacity(desired_spots)
        );
        Then(
            Box_created_with_capacity(actual_spots)
        );
    }

    protected CreateBox Create_box_for_capacity(int desiredNumberOfSpots)
    {
        return new CreateBox(Box_Id, desiredNumberOfSpots);
    }
}