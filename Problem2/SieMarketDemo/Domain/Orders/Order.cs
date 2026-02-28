using System;
using System.Collections.Generic;
using System.Linq;
using SieMarket.Domain.Customers;
using SieMarket.Domain.Discounts;

namespace SieMarket.Domain.Orders;

public sealed class Order
{
    private readonly List<OrderItem> _items = new();

    public Order(OrderId id, Customer customer)
    {
        Id = id;
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
    }

    public OrderId Id { get; }
    public Customer Customer { get; }

    public IReadOnlyList<OrderItem> Items => _items;

    public void AddItem(OrderItem item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        _items.Add(item);
    }

    // 2.1 / 2.2
    public decimal GetSubtotal() => _items.Sum(i => i.TotalPrice);

    public decimal CalculateFinalPrice(IOrderDiscountPolicy? discountPolicy = null)
    {
        if (_items.Count == 0)
            throw new InvalidOperationException("An order must contain at least one item.");

        discountPolicy ??= new ThresholdDiscountPolicy(500m, 0.10m);

        var subtotal = GetSubtotal();
        return discountPolicy.Apply(subtotal);
    }
}