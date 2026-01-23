using FluentValidation;
using OrderService.WebApi.Controllers;

namespace OrderService.WebApi.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        // Валидация ProductId
        RuleFor(request => request.ProductId)
            .NotEmpty().WithMessage("Product ID is required.")
            .GreaterThan(0).WithMessage("Product ID must be a positive number.");

        // Валидация Amount
        RuleFor(request => request.Amount)
            .NotEmpty().WithMessage("Amount is required.")
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        // Валидация EmailClient
        RuleFor(request => request.EmailClient)
            .NotEmpty().WithMessage("Client email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        // Валидация Price
        RuleFor(request => request.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        // Валидация PhoneNumber
        RuleFor(request => request.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("A valid phone number is required.");
    }
}
