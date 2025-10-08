using CartAPI.DTOs.Requests;
using CartAPI.DTOs.Responses;

namespace CartAPI.Services.Interfaces;

public interface ICartService
{
    Task AddItemToCart(int itemId, int userId);
    Task<CartResponse> GetCartAsync(int userId);
    Task ReplaceCart(ReplaceCartRequest replaceCartRequest);
}
