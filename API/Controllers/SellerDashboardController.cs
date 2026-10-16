using Application.DTOs.SellerDashboardDto;
using Application.Interfaces;
using Application.Interfaces.Services.Dashboards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Seller")]
public class SellerDashboardController : ControllerBase
{
    private readonly ISellerDashboardService _sellerDashboardService;

    public SellerDashboardController(
        ISellerDashboardService sellerDashboardService)
    {
        _sellerDashboardService = sellerDashboardService;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview(Guid storeId)
    {
        var result =
            await _sellerDashboardService.GetOverview(storeId);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpGet("details/orders")]
    public async Task<IActionResult> GetOrdersDetails(
        [FromQuery] SellerDashboardOrdersRequest request)
    {
        var result =
            await _sellerDashboardService.GetOrdersDetails(request);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpGet("details/turnover")]
    public async Task<IActionResult> GetTurnoverDetails(
        [FromQuery] SellerDashboardTurnoverRequest request)
    {
        var result =
            await _sellerDashboardService.GetTurnoverDetails(request);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpGet("details/settlement")]
    public async Task<IActionResult> GetSettlementDetails(
        [FromQuery] Guid storeId)
    {
        var result =
            await _sellerDashboardService.GetSettlementDetails(storeId);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpGet("details/products")]
    public async Task<IActionResult> GetProductDetails(
        [FromQuery] Guid storeId)
    {
        var result =
            await _sellerDashboardService.GetProductDetails(storeId);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }
}