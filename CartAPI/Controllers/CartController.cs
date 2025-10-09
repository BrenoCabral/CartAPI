using CartAPI.DTOs.Requests;
using CartAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CartAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController(ICartService cartService, ILogger<CartController> _logger) : ControllerBase
{
    private readonly ICartService _cartService = cartService;
    private static readonly ActivitySource ActivitySource = new("CartAPI.Controllers");
    [HttpPost]
    public async Task<IActionResult> GetCart(GetCartRequest getCartRequest)
    {
        var cart = await _cartService.GetCartAsync(getCartRequest.UserId);
        return Ok(cart);
    }
    [HttpPost("add")]

    public async Task<IActionResult> AddItemToCart(AddItemRequest request)
    {
        var userId = GetUserId();
        await _cartService.AddItemToCart(request.ItemId, userId);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Put(ReplaceCartRequest replaceCartRequest)
    {
        await _cartService.ReplaceCart(replaceCartRequest);
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        using var activity = ActivitySource.StartActivity("GetCart");

        try
        {
            var userId = GetUserId();
            _logger.LogInformation("Fetching cart for user: {UserId}", userId);

            // Add custom tags to the trace
            activity?.SetTag("user.id", userId);
            activity?.SetTag("operation", "GetCart");
            var cart = await _cartService.GetCartAsync(userId);

            _logger.LogInformation("Successfully retrieved cart for user: {UserId}", userId);
            return Ok(cart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cart for user: {UserId}", User.Identity?.Name);

            activity?.AddException(ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            return StatusCode(500, new { error = "An error occurred while retrieving the cart" });
        }
    }

    private int GetUserId()
    {
        var userId = User.FindFirstValue("Id");

        return userId == null ? throw new UnauthorizedAccessException("User ID not found in token.") : int.Parse(userId);
    }
}
