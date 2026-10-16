using Application.DTOs.ReturnRequestDto;
using Application.Interfaces.Services;
using Application.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReturnRequestController : ControllerBase
{
    private readonly IReturnRequestService _returnRequestService;

    public ReturnRequestController(
        IReturnRequestService returnRequestService)
    {
        _returnRequestService = returnRequestService;
    }

    // Buyer: create a return request
    [HttpPost]
    [Authorize(Roles = "Buyer")]
    public async Task<IActionResult> CreateReturnRequest(
        [FromBody] CreateReturnRequestDto request)
    {
        var result = await _returnRequestService
            .CreateReturnRequest(request);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    // Buyer: get my return requests
    [HttpGet("my")]
    [Authorize(Roles = "Buyer")]
    public async Task<IActionResult> GetMyReturnRequest(
        [FromQuery] ReturnRequestPaginationRequest request)
    {
        var result = await _returnRequestService
            .GetMyReturnRequest(request);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    // Seller: get return requests for my store
    [HttpGet("seller")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> GetSellerReturnRequest(
        [FromQuery] ReturnRequestPaginationRequest request)
    {
        var result = await _returnRequestService
            .GetSellerReturnRequest(request);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    // Admin: get all return requests
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetReturnRequestForAdmins(
        [FromQuery] ReturnRequestPaginationRequest request)
    {
        var result = await _returnRequestService
            .GetReturnRequestForAdmins(request);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    // Seller: mark returned product as received and process refund
    [HttpPatch("{returnRequestId:guid}/received")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> MarkAsReturned(Guid returnRequestId)
    {
        var result = await _returnRequestService
            .MarkAsReturned(returnRequestId);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("cancel-order/{storeOrderId:guid}")]
    public async Task<IActionResult> CancelStoreOrder(Guid storeOrderId)
    {
        var result = await _returnRequestService.CancelStoreOrderAsync(storeOrderId);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}