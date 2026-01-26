using MediatR;
using OrderService.WebApi.Controllers;

namespace OrderService.WebApi.UseCases.Commands;
public record GetOrderQuery(long OrderId) : IRequest<GetOrderResponse?>;

