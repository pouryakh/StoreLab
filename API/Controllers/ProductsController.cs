using Application.DTOs.ProductDto;
using Application.Interfaces.Services;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductCatalogService _productCatalogService;

        private readonly IProductService _productService;

        public ProductsController(IProductCatalogService productCatalogService,IProductService productService)
        {
            _productCatalogService = productCatalogService;
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]

        // -------------- product catalog --------------- //
        public async Task<IActionResult> GetProducts(
        [FromQuery] ProductQueryRequest request)
        {
            var result = await _productCatalogService.GetProductsAsync(request);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result.Data);
        }

        [HttpGet("{productId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(Guid productId)
        {
            var result = await _productCatalogService.GetProductById(productId);

            if (!result.IsSuccess)
                return NotFound(new { message = result.Error });

            return Ok(result.Data);
        }

        // ------------------- Product Services ---------------- //

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequestDto request)
        {
            var result = await _productService.CreateProductAsync(request);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result);
        }

        [HttpPut("{productId:guid}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateProduct(
            Guid productId,
            [FromBody] UpdateProductRequest request
            )
        {
            var result = await _productService.UpdateProductAsync(productId, request);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result);
        }

        [HttpPatch("{productId:guid}/activate")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> ActivateProduct(Guid productId)
        {
            var result = await _productService.ActivateProductAsync(productId);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result);
        }

        [HttpPatch("{productId:guid}/deactivate")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeactivateProduct(Guid productId)
        {
            var result = await _productService.DeactivateProductAsync(productId);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result);
        }

        [HttpPost("change-stock")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> ChangeStock([FromBody] ChangeStockRequest request)
        {
            var result = await _productService.ChangeStockAsync(request);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result);
        }


    }
}
