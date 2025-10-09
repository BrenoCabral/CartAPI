using BCrypt.Net;
using CartAPI.Data;
using CartAPI.DTOs.Requests;
using CartAPI.Models;
using CartAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext _db, IJwtServices _jwt, IUserService _userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _userService.UserExists(request.Email))
            return BadRequest("Email already in use.");

        await _userService.CreateNewUser(request);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userService.GetUserByEmail(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials.");

        var token = _jwt.GenerateToken(user);
        return Ok(new { Token = token });
    }
}
