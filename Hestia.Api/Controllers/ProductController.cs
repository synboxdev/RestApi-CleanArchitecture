using Hestia.Application.Handlers.Product.Commands.CreateProduct;
using Hestia.Application.Handlers.Product.Commands.DeleteProduct;
using Hestia.Application.Handlers.Product.Commands.UpdateProduct;
using Hestia.Application.Handlers.Product.Queries.GetProduct;
using Hestia.Application.Interfaces.Infrastructure;
using Hestia.Application.Models.Shared;
using Hestia.Domain.Models.Product.Inbound.CreateProduct;
using Hestia.Domain.Models.Product.Inbound.GetProduct;
using Hestia.Domain.Models.Product.Inbound.UpdateProduct;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hestia.Api.Controllers;

/// <summary>
/// Product related endpoints provide full functionality for management of products and product related details
/// </summary>
[Authorize(Roles = "User, Admin")]
[Route("api/[controller]")]
[ApiController]
public class ProductController(ICoreLayer coreLayer) : ControllerBase
{
    /// <summary>
    /// Retrieves a product by either Id or ExternalId
    /// </summary>
    [HttpGet("Get")]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProductAsync([FromQuery] GetProductDto request, CancellationToken cancellationToken = default)
    {
        var result = await coreLayer.ExecuteAsync(new GetProductQuery(request), cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    /// <summary>
    /// Creates a new product with provided details
    /// </summary>
    [HttpPost("Create")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProductAsync(CreateProductDto request, CancellationToken cancellationToken = default)
    {
        var result = await coreLayer.ExecuteAsync(new CreateProductCommand(request, FetchTokenFromRequest()), cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    /// <summary>
    /// Updates an existing product with provided details
    /// </summary>
    [HttpPut("Update")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProductAsync(UpdateProductDto request, CancellationToken cancellationToken = default)
    {
        var result = await coreLayer.ExecuteAsync(new UpdateProductCommand(request), cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    /// <summary>
    /// Deletes a product by either Id or ExternalId
    /// </summary>
    [HttpDelete("Delete")]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProductAsync([FromBody] GetProductDto request, CancellationToken cancellationToken = default)
    {
        var result = await coreLayer.ExecuteAsync(new DeleteProductCommand(request), cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    private string FetchTokenFromRequest() => HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
}