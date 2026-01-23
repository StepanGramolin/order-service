using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.Contracts;
using OrderService.WebApi.Controllers;
using Riok.Mapperly.Abstractions;

namespace OrderService.WebApi.Mappers
{
    [Mapper]
    public partial class OrderMapper
    {
        // 1. Маппинг из сущности Order (из БД) в DTO OrderCreatedV1 (для Kafka)
        // Это используется в Create() для отправки события.
        public partial OrderCreatedV1 ToOrderCreatedV1(Order order);

        // 2. Маппинг из DTO CreateOrderRequest (из тела запроса) в сущность Order (для БД)
        // Это используется в Create() для сохранения в базу.
        public partial Order ToOrderEntity(CreateOrderRequest request);

        // 3. Маппинг из сущности Order (из БД) в DTO GetOrderResponse (для GET запроса)
        // Это используется в Get() для возврата данных клиенту.
        public partial GetOrderResponse ToGetOrderResponse(Order order);
    }
}
