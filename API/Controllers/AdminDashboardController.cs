using Application.Interfaces.Services.Dashboards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminDashboardController(
            IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        // GET: api/AdminDashboard/overview
        [HttpGet("overview")]
        public async Task<IActionResult> GetAdminDashboardOverview()
        {
            var result =
                await _adminDashboardService.GetAdminDashboardOverview();

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result.Data);
        }

        // GET: api/AdminDashboard/orders
        [HttpGet("orders")]
        public async Task<IActionResult> GetAdminDashboardOrdersDetails()
        {
            var result =
                await _adminDashboardService.GetAdminDashboardOrderDetails();

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result.Data);
        }

        // GET: api/AdminDashboard/finance
        [HttpGet("finance")]
        public async Task<IActionResult> GetAdminDashboardFinanceDetails()
        {
            var result =
                await _adminDashboardService.GetAdminDashboardFinanceDetails();

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result.Data);
        }

        // GET: api/AdminDashboard/stores
        [HttpGet("stores")]
        public async Task<IActionResult> GetAdminDashboardStoresDetails()
        {
            var result =
                await _adminDashboardService.GetAdminDashboardStoresDetails();

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result.Data);
        }
    }
}