using FluentValidation;
using OrderService.WebApi.UseCases.Commands;

namespace OrderService.WebApi.Validators;
public class GetOrderQueryValidator : AbstractValidator<GetOrderQuery>
{
    public GetOrderQueryValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0);
    }
}
