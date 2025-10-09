namespace CartAPI.DTOs.Responses;

public record CartItemResponse(int UserId, int ItemId, decimal Price, int Quantity = 1);