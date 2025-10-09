namespace CartAPI.Models;

public class User { 
    public int Id { get; set; } 
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public ICollection<CartItem> CartItems { get; set; } 
}
