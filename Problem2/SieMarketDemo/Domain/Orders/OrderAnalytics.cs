using System;
using System.Collections.Generic;
using System.Linq;
using SieMarket.Domain.Discounts;

namespace SieMarket.Domain.Orders;

public static class OrderAnalytics
{
    // 2.3
    public static string GetTopSpenderCustomerName(
        IEnumerable<Order> orders,
        IOrderDiscountPolicy? discountPolicy = null)
    {
        if (orders is null) throw new ArgumentNullException(nameof(orders));

        discountPolicy ??= new ThresholdDiscountPolicy(500m, 0.10m);

        var best = orders
            .GroupBy(o => new { o.Customer.Id, o.Customer.Name })
            .Select(g => new
            {
                g.Key.Name,
                Total = g.Sum(o => o.CalculateFinalPrice(discountPolicy))
            })
            .OrderByDescending(x => x.Total)
            .FirstOrDefault();

        return best?.Name ?? string.Empty;
    }

    // 2.4 Bonus - rezultat
    public sealed record ProductSales(string ProductName, int TotalQuantitySold);

    // 2.4 Bonus
    public static IReadOnlyList<ProductSales> GetPopularProductsWithTotalQuantitySold(
        IEnumerable<Order> orders)
    {
        if (orders is null) throw new ArgumentNullException(nameof(orders));

        return orders
            .SelectMany(o => o.Items)
            .GroupBy(i => i.ProductName, StringComparer.OrdinalIgnoreCase)
            .Select(g => new ProductSales(g.Key, g.Sum(i => i.Quantity)))
            .OrderByDescending(x => x.TotalQuantitySold)
            .ThenBy(x => x.ProductName)
            .ToList();
    }
}