using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres.AppDbContext;
using OrderService.WebApi.Infrastructure;
using OrderService.WebApi.Mappers;

namespace OrderService.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly OrdersDbContext _db;
    private readonly PaymentsClient _payments;
    private readonly KafkaProducer _producer;
    private readonly OrderMapper _orderMapper;


    public OrdersController(OrdersDbContext db, PaymentsClient payments, 
        KafkaProducer producer, OrderMapper orderMapper)
    {
        _db = db;
        _payments = payments;
        _producer = producer;
        _orderMapper = orderMapper;
    }

    // POST api/orders/create
    [HttpPost("create")]
    public async Task<ActionResult<long>> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var order = _orderMapper.ToOrderEntity(request);

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct); // генерируется order.OrderId

        // orderId заказа == orderId платежа
        await _payments.CreatePaymentAsync(
            new CreatePaymentRequest(order.OrderId, order.Price),
            ct
        );

        var correlationId =
            Request.Headers.TryGetValue("X-Correlation-Id", out var cid) && !string.IsNullOrWhiteSpace(cid)
                ? cid.ToString()
                : Guid.NewGuid().ToString("N");

        var evt = _orderMapper.ToOrderCreatedV1(order);

        await _producer.ProducePaymentSucceededAsync(evt, correlationId, ct);

        // Возвращаем только идентификатор заказа
        return Ok(order.OrderId);
    }

    // GET api/orders/{order_id}
    [HttpGet("{orderId:long}")]
    public async Task<ActionResult<GetOrderResponse>> Get(long orderId, CancellationToken ct)
    {
        var order = await _db.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.OrderId == orderId, ct);
        if (order is null) return NotFound();

        return Ok(_orderMapper.ToGetOrderResponse(order));
    }

    // DELETE api/orders/{order_id}
    [HttpDelete("{orderId:long}")]
    public async Task<IActionResult> Delete(long orderId, CancellationToken ct)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId, ct);
        if (order is null) return NotFound();

        _db.Orders.Remove(order);
        await _db.SaveChangesAsync(ct);

        return NoContent();
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