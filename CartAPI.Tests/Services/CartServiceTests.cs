using CartAPI.Data;
using CartAPI.DTOs.Requests;
using CartAPI.Models;
using CartAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Tests.Services;

public class CartServiceTests
{
    private static async Task<AppDbContext> GetDbContextAsync(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var context = new AppDbContext(options);

        // Seed base data
        var user = new User { Id = 1, Name = "John", Email = "john@test.com", PasswordHash = "hash" };
        var item1 = new Item { Id = 1, Name = "Apple", Price = 2.0m };
        var item2 = new Item { Id = 2, Name = "Banana", Price = 3.0m };

        context.Users.Add(user);
        context.Items.AddRange(item1, item2);
        await context.SaveChangesAsync();

        return context;
    }

    [Fact]
    public async Task AddItemToCart_ShouldAddNewItem_WhenNotExists()
    {
        var context = await GetDbContextAsync(nameof(AddItemToCart_ShouldAddNewItem_WhenNotExists));
        var service = new CartService(context);

        await service.AddItemToCart(1, 1);

        var cartItem = await context.Carts.FirstAsync();
        Assert.Equal(1, cartItem.Quantity);
        Assert.Equal(1, cartItem.ItemId);
        Assert.Equal(1, cartItem.UserId);
    }

    [Fact]
    public async Task AddItemToCart_ShouldIncrementQuantity_WhenItemExists()
    {
        var context = await GetDbContextAsync(nameof(AddItemToCart_ShouldIncrementQuantity_WhenItemExists));
        var service = new CartService(context);

        context.Carts.Add(new CartItem { ItemId = 1, UserId = 1, Quantity = 1 });
        await context.SaveChangesAsync();

        await service.AddItemToCart(1, 1);

        var cartItem = await context.Carts.FirstAsync();
        Assert.Equal(2, cartItem.Quantity);
    }

    [Fact]
    public async Task GetCartAsync_ShouldReturnCartResponse()
    {
        var context = await GetDbContextAsync(nameof(GetCartAsync_ShouldReturnCartResponse));
        var service = new CartService(context);

        context.Carts.Add(new CartItem { UserId = 1, ItemId = 1, Quantity = 2 });
        await context.SaveChangesAsync();

        var result = await service.GetCartAsync(1);

        Assert.Equal("John", result.Name);
        Assert.Single(result.Cart);
        Assert.Equal(2, result.Cart.First().Quantity);
    }

    [Fact]
    public async Task AddItemToCart_ShouldThrow_WhenItemNotFound()
    {
        var context = await GetDbContextAsync(nameof(AddItemToCart_ShouldThrow_WhenItemNotFound));
        var service = new CartService(context);

        await Assert.ThrowsAsync<ArgumentException>(() => service.AddItemToCart(999, 1));
    }

}
