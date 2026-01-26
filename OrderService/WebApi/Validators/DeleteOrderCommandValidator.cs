using FluentValidation;
using OrderService.WebApi.UseCases.Commands;

namespace OrderService.WebApi.Validators;
public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0);
    }
}