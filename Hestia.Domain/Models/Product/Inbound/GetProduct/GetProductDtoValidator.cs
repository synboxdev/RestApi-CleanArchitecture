using FluentValidation;

namespace Hestia.Domain.Models.Product.Inbound.GetProduct;

public class GetProductDtoValidator : AbstractValidator<GetProductDto>
{
    public GetProductDtoValidator()
    {
        RuleFor(x => x)
            .Must(HaveAtLeastOneField)
            .WithMessage("At least one of 'Id' or 'ExternalId' must be provided.");
    }

    private static bool HaveAtLeastOneField(GetProductDto dto) => dto.Id != Guid.Empty || !string.IsNullOrEmpty(dto.ExternalId);
}