using System;

namespace SieMarket.Domain.Discounts;

public sealed class ThresholdDiscountPolicy : IOrderDiscountPolicy
{
    private readonly decimal _threshold;
    private readonly decimal _discountRate;

    public ThresholdDiscountPolicy(decimal threshold = 500m, decimal discountRate = 0.10m)
    {
        if (threshold < 0) throw new ArgumentOutOfRangeException(nameof(threshold));
        if (discountRate < 0 || discountRate > 1) throw new ArgumentOutOfRangeException(nameof(discountRate));

        _threshold = threshold;
        _discountRate = discountRate;
    }

    public decimal Apply(decimal subtotal)
    {
        if (subtotal > _threshold)
            return subtotal * (1m - _discountRate);

        return subtotal;
    }
}