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
    public async Task<IActionResult> CreateBox([FromBody]CreateBox command)
    {
        // You should probably map your external contracts.
        await router.HandleCommand(command);
        return Accepted();
    }
    
    [HttpPost]
    [Route("add-bottle")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> AddBottle([FromBody]AddBeerBottle command)
    {
        await router.HandleCommand(command);
        return Accepted();
    }
    
    [HttpPost]
    [Route("add-label")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> AddLabel([FromBody]AddShippingLabel command)
    {
        await router.HandleCommand(command);
        return Accepted();
    }
    
    [HttpPost]
    [Route("close")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> CloseBox([FromBody]CloseBox command)
    {
        await router.HandleCommand(command);
        return Accepted();
    }
    
    [HttpPost]
    [Route("send")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SendBox([FromBody]SendBox command)
    {
        await router.HandleCommand(command);
        return Accepted();
    }
}