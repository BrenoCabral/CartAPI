using CartAPI.Models;

namespace CartAPI.Services.Interfaces;

public interface IJwtServices
{
    string GenerateToken(User user);
}