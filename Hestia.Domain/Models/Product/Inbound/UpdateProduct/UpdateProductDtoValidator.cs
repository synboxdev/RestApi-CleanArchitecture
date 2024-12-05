using FluentValidation;

namespace Hestia.Domain.Models.Product.Inbound.UpdateProduct;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required. This has been provided during the creation of a given product.");

        RuleFor(x => x.ExternalId)
            .NotNull().NotEmpty().WithMessage("External product id is required. This should be a unique code for each.");

        RuleFor(x => x.Name)
            .NotNull().NotEmpty().WithMessage("Product name may not be null or empty!")
            .MinimumLength(3).WithMessage("Product name must be at least 3 characters long.");

        RuleFor(x => x.Description)
            .NotNull().NotEmpty().WithMessage("Product description may not be null or empty.")
            .MinimumLength(3).WithMessage("Product description must be at least 3 characters long.");

        RuleFor(x => x.Price)
            .NotNull()
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}