using Application.DTOs.ShoppingCartDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var result = await _shoppingCartService.GetShoppingCart();

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddCartItem(
            [FromBody] AddItemRequest request)
        {
            var result = await _shoppingCartService.AddItem(request);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItemQuantity(
            [FromBody] UpdateItemQuantityRequest request)
        {
            var result = await _shoppingCartService.UpdateItemQuantity(request);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveCartItem(
            [FromBody] RemoveItemRequest request)
        {
            var result = await _shoppingCartService.RemoveItem(request);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _shoppingCartService.ClearShoppingCart();

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
