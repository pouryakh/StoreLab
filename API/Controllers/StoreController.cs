using Application.DTOs.StoreDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateStore(CreateStoreRequest request)
        {
            var result = await _storeService.CreateStore(request);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result);
        }

        [HttpGet("my-stores")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetMyStores()
        {
            var result = await _storeService.GetMyStores();

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result);
        }

        [HttpGet("{storeId:guid}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetStoreById(Guid storeId)
        {
            var result = await _storeService.GetStoreById(storeId);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result);
        }

        [HttpPut("{storeId:guid}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateStore(
            Guid storeId,
            [FromBody] UpdateStoreRequest request)
        {
            var result = await _storeService.UpdateStore(request, storeId);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result);
        }

        [HttpPatch("{storeId:guid}/deactivate")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeactivateStore(Guid storeId)
        {
            var result = await _storeService.DeactivateStore(storeId);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result);
        }

        [HttpPost("{storeId:guid}/activation-request")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> SendActivationRequest(Guid storeId)
        {
            var result = await _storeService.SendActivationRequest(storeId);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(result);
        }
    }
}