using Application.DTOs.CommonDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class StoreActivationRequestController : ControllerBase
    {
        private readonly IStoreActivationRequestService _activationRequestService;

        public StoreActivationRequestController(
            IStoreActivationRequestService activationRequestService)
        {
            _activationRequestService = activationRequestService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingActivationRequests([FromQuery] PaginationRequest request)
        {
            var result =
                await _activationRequestService
                    .GetPendingActivationRequests(request);

            if (!result.IsSuccess)
                return BadRequest(new
                {
                    message = result.Error
                });

            return Ok(result.Data);
        }

        [HttpPost("{activationRequestId:guid}/approve")]
        public async Task<IActionResult> ApproveActivationRequest(Guid activationRequestId, [FromBody] string? note)
        {
            var result =
                await _activationRequestService
                    .ApproveActivationRequest(
                        activationRequestId,
                        note);

            if (!result.IsSuccess)
                return BadRequest(new
                {
                    message = result.Error
                });

            return Ok(result);
        }

        [HttpPost("{activationRequestId:guid}/reject")]
        public async Task<IActionResult> RejectActivationRequest(Guid activationRequestId, [FromBody] string? note)
        {
            var result =
                await _activationRequestService
                    .RejectActivationRequest(
                        activationRequestId,
                        note);

            if (!result.IsSuccess)
                return BadRequest(new
                {
                    message = result.Error
                });

            return Ok(result);
        }
    }
}