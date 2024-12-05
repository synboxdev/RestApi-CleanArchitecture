using AutoMapper;
using Hestia.Domain.Models.Product.Inbound.GetProduct;
using Hestia.Domain.Models.Product.Inbound.UpdateProduct;

namespace Hestia.Application.Profiles.Product;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<UpdateProductDto, Access.Entities.Product.Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ExternalId, opt => opt.Ignore())
            .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.DateEdited, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Access.Entities.Product.Product, GetProductResponseDto>();
    }
}