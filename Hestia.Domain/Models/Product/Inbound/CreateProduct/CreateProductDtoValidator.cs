using FluentValidation;

namespace Hestia.Domain.Models.Product.Inbound.CreateProduct;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.ExternalId)
            .NotNull().NotEmpty().WithMessage("External product id is required. This should be a unique code for each.");

        RuleFor(x => x.Name)
            .NotNull().NotEmpty().WithMessage("Product name is required.")
            .MinimumLength(3).WithMessage("Product name must be at least 3 characters long.");

        RuleFor(x => x.Description)
            .NotNull().NotEmpty().WithMessage("Product description is required.")
            .MinimumLength(3).WithMessage("Product description must be at least 3 characters long.");

        RuleFor(x => x.Price)
            .NotNull().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}