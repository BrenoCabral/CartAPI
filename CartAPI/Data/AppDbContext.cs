using CartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Items { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<CartItem> Carts { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>()
            .Property(i => i.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<CartItem>()
            .Property(ci => ci.Id)
            .ValueGeneratedOnAdd();
    }
}
