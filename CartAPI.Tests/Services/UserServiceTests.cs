using CartAPI.Data;
using CartAPI.DTOs.Requests;
using CartAPI.Models;
using CartAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Tests.Services;

public class UserServiceTests
{
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique per test
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateNewUser_ShouldAddUser_WhenValid()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var service = new UserService(context);
        var request = new RegisterRequest("John Doe", "john@example.com", "Password123!");

        // Act
        var user = await service.CreateNewUser(request);

        // Assert
        Assert.True(user);
    }

    [Fact]
    public async Task CreateNewUser_ShouldThrow_WhenEmailAlreadyExists()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var service = new UserService(context);
        var email = "duplicate@example.com";

        context.Users.Add(new User { Name = "Existing", Email = email, PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var request = new RegisterRequest("New User", email, "Secret123");

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateNewUser(request));
    }

    [Fact]
    public async Task UserExists_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var service = new UserService(context);
        var email = "exists@example.com";

        context.Users.Add(new User { Name = "User", Email = email, PasswordHash = "hash" });
        await context.SaveChangesAsync();

        // Act
        var exists = await service.UserExists(email);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task UserExists_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var service = new UserService(context);

        // Act
        var exists = await service.UserExists("notfound@example.com");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task GetUserByEmail_ShouldReturnUser_WhenExists()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var service = new UserService(context);
        var email = "findme@example.com";

        var user = new User { Name = "Target", Email = email, PasswordHash = "hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetUserByEmail(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result!.Email);
    }

    [Fact]
    public async Task GetUserByEmail_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var service = new UserService(context);

        // Act
        var result = await service.GetUserByEmail("missing@example.com");

        // Assert
        Assert.Null(result);
    }
}