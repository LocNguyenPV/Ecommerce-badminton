namespace ECommerce.Application.DTOs;

public record ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public IEnumerable<string>? Errors { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
