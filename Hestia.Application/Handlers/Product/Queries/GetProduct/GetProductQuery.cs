﻿using Hestia.Application.Models.Shared;
using Hestia.Domain.Models.Product.Inbound.GetProduct;
using MediatR;

namespace Hestia.Application.Handlers.Product.Queries.GetProduct;

public sealed record GetProductQuery(GetProductDto Product) : IRequest<ApiResponse<GetProductResponseDto?>>;