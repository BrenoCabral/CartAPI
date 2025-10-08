using CartAPI.Data;
using CartAPI.DTOs.Requests;
using CartAPI.DTOs.Responses;
using CartAPI.Models;
using CartAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Services;
public class CartService(AppDbContext _context) : ICartService
{
    public async Task AddItemToCart(int itemId, int userId)
    {
        var item = await _context.Items.FindAsync(itemId) ?? throw new ArgumentException($"Item with ID {itemId} not found.");

        var user = await GetUser(userId);

        var existingCartItem = await _context.Carts
            .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ItemId == itemId);

        if (existingCartItem != null)
        {
            existingCartItem.Quantity += 1;
            _context.Carts.Update(existingCartItem);
        }
        else
        {
            var newCartItem = new CartItem
            {
                UserId = userId,
                ItemId = itemId,
                Quantity = 1,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Carts.AddAsync(newCartItem);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<CartResponse> GetCartAsync(int userId)
    {
        var user = await GetUser(userId);

        var cartItems = _context.Carts
            .Where(c => c.UserId == userId && string.IsNullOrEmpty(c.CartSnapshotId))
            .Include(c => c.Item)
            .Select(c => new CartItemResponse(userId, c.ItemId, c.Item.Price, c.Quantity));

        return new CartResponse(user.Name, cartItems);
    }

    public async Task ReplaceCart(ReplaceCartRequest replaceCartRequest)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var existingCart = _context.Carts
            .Where(c => c.UserId == replaceCartRequest.UserId && string.IsNullOrEmpty(c.CartSnapshotId));

            var snapshotId = Guid.NewGuid().ToString();

            await existingCart.ForEachAsync(c => c.CartSnapshotId = snapshotId);

            if (replaceCartRequest.ItemIds != null && replaceCartRequest.ItemIds.Any())
            {
                var itemQuantities = replaceCartRequest.ItemIds
                    .GroupBy(id => id)
                    .ToDictionary(g => g.Key, g => g.Count());

                var itemIds = itemQuantities.Keys;

                var items = _context.Items
                    .Where(i => itemIds.Contains(i.Id));

                var user = await GetUser(replaceCartRequest.UserId);

                var newCartItems = items.Select(i => new CartItem
                {
                    Item = i,
                    User = user,
                    Quantity = itemQuantities[i.Id],
                });

                await _context.Carts.AddRangeAsync(newCartItems);
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<User> GetUser(int userId) =>
        await _context.Users.FindAsync(userId) ?? throw new ArgumentException($"User with ID {userId} not found.");
}