using Application.DTOs.WalletDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserWallet()
    {
        var result = await _walletService.GetUserWallet();

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> DepositToWallet([FromBody] DepositRequest request)
    {
        var result = await _walletService.DepositToWallet(request);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] WalletPaginationRequest request)
    {
        var result = await _walletService.GetTransactions(request);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpGet("transactions/{transactionId:guid}")]
    public async Task<IActionResult> GetTransactionById(Guid transactionId)
    {
        var result =
            await _walletService.GetTransactionById(transactionId);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }
}