namespace SieMarket.Domain.Discounts;

public interface IOrderDiscountPolicy
{
    decimal Apply(decimal subtotal);
}