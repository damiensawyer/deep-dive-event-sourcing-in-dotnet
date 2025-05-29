using BeerSender.Domain;
using BeerSender.Domain.Boxes.Commands;
using Microsoft.AspNetCore.Mvc;

namespace BeerSender.Web.Controllers;

[ApiController]
[Route("api/command/[controller]")]
public class BoxController(CommandRouter router) : ControllerBase
{
    [HttpPost]
    [Route("create")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult CreateBox([FromBody]CreateBox command)
    {
        // You should probably map your external contracts.
        router.HandleCommand(command);
        return Accepted();
    }
    
    [HttpPost]
    [Route("add-bottle")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult AddBottle([FromBody]AddBeerBottle command)
    {
        router.HandleCommand(command);
        return Accepted();
    }
    
    [HttpPost]
    [Route("add-label")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult AddLabel([FromBody]AddShippingLabel command)
    {
        router.HandleCommand(command);
        return Accepted();
    }
    
    [HttpPost]
    [Route("close")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult CloseBox([FromBody]CloseBox command)
    {
        router.HandleCommand(command);
        return Accepted();
    }
    
    [HttpPost]
    [Route("send")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult SendBox([FromBody]SendBox command)
    {
        router.HandleCommand(command);
        return Accepted();
    }
}
