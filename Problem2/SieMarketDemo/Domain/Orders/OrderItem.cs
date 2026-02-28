using System;

namespace SieMarket.Domain.Orders;

public sealed class OrderItem
{
    public OrderItem(string productName, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty.", nameof(productName));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be > 0.");
        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price must be >= 0.");

        ProductName = productName.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public string ProductName { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }

    public decimal TotalPrice => Quantity * UnitPrice;
}