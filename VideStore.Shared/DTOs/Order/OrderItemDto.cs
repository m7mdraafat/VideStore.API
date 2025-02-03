namespace VideStore.Shared.DTOs.Order
{
    public record OrderItemDto(
        string ProductId,
        string ProductName,
        decimal UnitPrice,
        int Quantity,
        string PictureUrl);
}
