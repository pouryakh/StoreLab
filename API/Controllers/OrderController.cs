using Application.DTOs.OrderDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery]OrdersPaginationRequest request)
        {
            var result =
                await _orderService.GetOrders(request);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result);
        }

        [HttpGet("{orderId:guid}")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var result =
                await _orderService.GetOrderById(orderId);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result);
        }
    }
}