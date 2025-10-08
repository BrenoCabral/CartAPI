namespace CartAPI.DTOs.Requests;

public record ReplaceCartRequest(int UserId, IEnumerable<int>? ItemIds);
