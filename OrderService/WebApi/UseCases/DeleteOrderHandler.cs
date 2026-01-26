using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres.AppDbContext;
using OrderService.WebApi.UseCases.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.WebApi.UseCases;
public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, bool>
{
    private readonly OrdersDbContext _db;

    public DeleteOrderHandler(OrdersDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken ct)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(x => x.OrderId == request.OrderId, ct);

        if (order == null) return false;

        _db.Orders.Remove(order);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}