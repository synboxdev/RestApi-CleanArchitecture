﻿using MediatR;

namespace Hestia.Access.Requests.Product.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Entities.Product.Product Product) : IRequest<bool>;