﻿using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.OrderEntities;

public class OrderItem : BaseEntity
{
    public OrderItem() { }

    public OrderItem(int orderId, int productId, string productName, string productImageCover ,int quantity, decimal unitPrice)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        ProductImageCover = productImageCover;
    }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductImageCover { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal Total => GetTotal();

    public decimal GetTotal() => UnitPrice * Quantity;
}