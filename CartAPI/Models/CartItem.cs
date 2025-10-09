using System.ComponentModel.DataAnnotations;

namespace CartAPI.Models;

public class CartItem
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int ItemId { get; set; }
    public Item Item { get; set; }
    public int Quantity { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CartSnapshotId { get; set; }
}
