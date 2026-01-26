using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres.AppDbContext;
using OrderService.WebApi.Controllers;
using OrderService.WebApi.Mappers;
using OrderService.WebApi.UseCases.Commands;

namespace OrderService.WebApi.UseCases;

public class GetOrderHandler : IRequestHandler<GetOrderQuery, GetOrderResponse?>
{
    private readonly OrdersDbContext _db;
    private readonly OrderMapper _mapper;

    public GetOrderHandler(OrdersDbContext db, OrderMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<GetOrderResponse?> Handle(GetOrderQuery request, CancellationToken ct)
    {
        var order = await _db.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OrderId == request.OrderId, ct);

        if (order == null) return null;

        return _mapper.ToGetOrderResponse(order);
    }
}
