using Azure.Core;
using CartAPI.Data;
using CartAPI.DTOs.Requests;
using CartAPI.Models;
using CartAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Services;

public class UserService(AppDbContext _context) : IUserService
{
    public async Task<bool> UserExists(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _context
            .Users
            .AsNoTracking()
            .AnyAsync(u => u.Email.ToLower() == normalized);
    }

    public async Task<bool> CreateNewUser(RegisterRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (await _context.Users.AnyAsync(u => u.Email.Equals(normalizedEmail, StringComparison.CurrentCultureIgnoreCase)))
            throw new InvalidOperationException("A user with this email already exists.");

        var user = new User
        {
            Name = request.Name,
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<User?> GetUserByEmail(string email) =>
    await _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
}
