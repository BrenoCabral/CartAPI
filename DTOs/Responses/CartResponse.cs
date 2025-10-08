namespace CartAPI.DTOs.Responses;

public record CartResponse(string Name, IEnumerable<CartItemResponse>? Cart);
