using MediatR;

namespace OrderService.WebApi.UseCases.Commands;

// IRequest<long> означает, что результатом выполнения этой команды будет ID заказа (long)
public record CreateOrderCommand(
    long ProductId,
    int Amount,
    string EmailClient,
    decimal Price,
    string PhoneNumber
) : IRequest<long>;