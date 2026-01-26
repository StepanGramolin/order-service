using MediatR;
using OrderService.WebApi.Infrastructure;
using OrderService.WebApi.Mappers;
using OrderService.DataAccess.Postgres.AppDbContext;
using OrderService.WebApi.UseCases.Commands;

namespace OrderService.WebApi.UseCases;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, long>
{
    private readonly OrdersDbContext _db;
    private readonly OrderMapper _mapper;
    private readonly PaymentsClient _payments;
    private readonly KafkaProducer _producer;

    public CreateOrderHandler(OrdersDbContext db, OrderMapper mapper, PaymentsClient payments, KafkaProducer producer)
    {
        _db = db;
        _mapper = mapper;
        _payments = payments;
        _producer = producer;
    }

    public async Task<long> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        // 1. Маппинг из Команды в Сущность (используем наш Mapperly)
        var order = _mapper.ToOrderEntity(request);

        // 2. Сохранение в БД
        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);

        // 3. Вызов платежного сервиса
        await _payments.CreatePaymentAsync(new CreatePaymentRequest(order.OrderId, order.Price), ct);

        // 4. Отправка в Kafka
        var evt = _mapper.ToOrderCreatedV1(order);
        await _producer.ProducePaymentSucceededAsync(evt, Guid.NewGuid().ToString("N"), ct);

        return order.OrderId;
    }
}