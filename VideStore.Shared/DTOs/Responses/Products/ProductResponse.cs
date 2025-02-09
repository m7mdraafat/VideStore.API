﻿namespace VideStore.Shared.DTOs.Responses.Products;

public class ProductResponse
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int StockQuantity { get; set; } = 0!;
    public decimal Price { get; set; }
    public double RatingsAverage { get; set; }
    public int Sold { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CategoryName { get; set; } = null!;
    public string ColorName { get; set; } = null!;
    public string ColorHexCode { get; set; } = null!;
    public List<string> ProductImageUrls { get; set; } = [];
    public List<ProductSizeResponse> ProductSizes { get; set; } = [];
}