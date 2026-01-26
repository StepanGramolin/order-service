using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.WebApi.UseCases.Commands;

namespace OrderService.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator) => _mediator = mediator;

    [HttpPost("create")]
    public async Task<ActionResult<long>> Create([FromBody] CreateOrderCommand command, CancellationToken ct)
    {
        // Контроллер просто отправляет команду в Mediator
        var orderId = await _mediator.Send(command, ct);
        return Ok(orderId);
    }

    // GET api/orders/{orderId}
    [HttpGet("{orderId:long}")]
    public async Task<ActionResult<GetOrderResponse>> Get(long orderId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrderQuery(orderId), ct);
        return result is null ? NotFound() : Ok(result);
    }

    // DELETE api/orders/{orderId}
    [HttpDelete("{orderId:long}")]
    public async Task<IActionResult> Delete(long orderId, CancellationToken ct)
    {
        var success = await _mediator.Send(new DeleteOrderCommand(orderId), ct);
        return success ? NoContent() : NotFound();
    }
}

public sealed record CreateOrderRequest(
    long ProductId,
    int Amount,
    string EmailClient,
    decimal Price,
    string PhoneNumber
);

public sealed record GetOrderResponse(
    long ProductId,
    int Amount,
    string EmailClient,
    decimal Price,
    string PhoneNumber
);