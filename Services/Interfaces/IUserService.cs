using CartAPI.DTOs.Requests;
using CartAPI.Models;

namespace CartAPI.Services.Interfaces;

public interface IUserService
{
    Task<bool> CreateNewUser(RegisterRequest request);
    Task<bool> UserExists(string email);
    Task<User?> GetUserByEmail(string email);
}