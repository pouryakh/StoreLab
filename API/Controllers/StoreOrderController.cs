using Application.DTOs.OrderDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StoreOrderController : ControllerBase
{
    private readonly IStoreOrderService _storeOrderService;

    public StoreOrderController(IStoreOrderService storeOrderService)
    {
        _storeOrderService = storeOrderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStoreOrders(
        [FromQuery] OrdersPaginationRequest request)
    {
        var result =
            await _storeOrderService.GetStoreOrders(request);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpGet("{storeOrderId:guid}")]
    public async Task<IActionResult> GetStoreOrderById(
        Guid storeOrderId)
    {
        var result =
            await _storeOrderService.GetStoreOrderById(storeOrderId);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpPatch("{storeOrderId:guid}/processing")]
    public async Task<IActionResult> MarkAsProcessing(
        Guid storeOrderId)
    {
        var result =
            await _storeOrderService.MarkAsProcessingAsync(storeOrderId);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpPatch("{storeOrderId:guid}/shipped")]
    public async Task<IActionResult> MarkAsShipped(
        Guid storeOrderId)
    {
        var result =
            await _storeOrderService.MarkAsShippedAsync(storeOrderId);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }
}