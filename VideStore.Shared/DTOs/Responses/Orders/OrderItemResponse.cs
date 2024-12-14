namespace VideStore.Shared.DTOs.Responses.Orders;

public class OrderItemResponse
{
    public string ProductName { get; set; } = null!;
    public string ProductImageCover { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}