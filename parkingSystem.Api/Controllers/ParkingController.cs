using Microsoft.AspNetCore.Mvc;
using parkingSystem.Api.DTOs;
using parkingSystem.Api.Services;

namespace parkingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParkingController : ControllerBase
{
    private readonly IParkingService _service;
    
    public ParkingController(IParkingService service) 
    { 
        _service = service; 
    }

    [HttpGet("occupancy")]
    public async Task<IActionResult> GetOccupancy() => Ok(await _service.GetOccupancySummaryAsync());

    [HttpGet("active-sessions")]
    public async Task<IActionResult> GetActiveSessions() => Ok(await _service.GetActiveSessionsAsync());

    [HttpPost("checkin")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
    {
        try
        {
            var ticket = await _service.CheckInAsync(request);
            return Ok(new { ticketNumber = ticket, message = "Check-in successful" });
        }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request)
    {
        try { return Ok(await _service.CheckOutAsync(request)); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("payments")]
    public async Task<IActionResult> GetPayments() => Ok(await _service.GetPaymentHistoryAsync());

    [HttpGet("daily-collections")]
    public async Task<IActionResult> GetDailyCollections() => Ok(await _service.GetDailyCollectionsAsync());

    [HttpGet("today-total")]
    public async Task<IActionResult> GetTodayTotal() => Ok(new { total = await _service.GetTodayTotalAsync() });

    [HttpGet("total-revenue")]
    public async Task<IActionResult> GetTotalRevenue() => Ok(new { total = await _service.GetTotalRevenueAsync() });

    [HttpGet("pricing-config")]
    public async Task<IActionResult> GetPricingConfig() => Ok(await _service.GetPricingConfigAsync());

    [HttpPost("pricing-config")]
    public async Task<IActionResult> SavePricingConfig([FromBody] PricingConfigRequest request)
    {
        try
        {
            await _service.SavePricingConfigAsync(request);
            return Ok(new { message = "Pricing configuration saved." });
        }
        catch (Exception ex) 
        { 
            return BadRequest(new { error = ex.Message }); 
        }
    }
}