using BeerSender.Domain;
using BeerSender.Domain.Boxes;
using BeerSender.QueryAPI.Database;
using Microsoft.AspNetCore.Mvc;

namespace BeerSender.QueryAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class BoxController(
    IEventStore eventStore, 
    BoxQueryRepository repository) : ControllerBase
{
    [HttpGet]
    [Route("{id:guid}")]
    public Box GetById([FromRoute]Guid id)
    {
        var boxStream = new EventStream<Box>(eventStore, id);
        var box = boxStream.GetEntity();
        return box;
    }

    [HttpGet]
    [Route("{id:guid}/by-sequence/{sequence:int}")]
    public Box GetById([FromRoute] Guid id, [FromRoute]int sequence)
    {
        var boxStream = new EventStream<Box>(eventStore, id);
        var box = boxStream.GetEntityBySequence(sequence);
        return box;
    }

    [HttpGet]
    [Route("all-open")]
    public IEnumerable<OpenBox> GetOpenBoxes()
    {
        return repository.GetAllOpen();
    }

    [HttpGet]
    [Route("all-unsent")]
    public IEnumerable<UnsentBox> GetUnsentBoxes()
    {
        return repository.GetAllUnsent();
    }
}