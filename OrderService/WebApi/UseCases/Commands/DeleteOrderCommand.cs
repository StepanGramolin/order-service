using MediatR;

namespace OrderService.WebApi.UseCases.Commands;

// Возвращаем bool, чтобы контроллер знал, удалось ли найти и удалить заказ
public record DeleteOrderCommand(long OrderId) : IRequest<bool>;
