using SieMarket.Application.Abstractions;
using SieMarket.Domain.Discounts;
using SieMarket.Domain.Orders;

namespace SieMarket.Application;

public sealed class OrderService
{
    private readonly IOrderRepository _repo;
    private readonly IOrderDiscountPolicy _discountPolicy;

    public OrderService(IOrderRepository repo, IOrderDiscountPolicy discountPolicy)
    {
        _repo = repo;
        _discountPolicy = discountPolicy;
    }

    public decimal CalculateFinalPrice(Order order) =>
        order.CalculateFinalPrice(_discountPolicy);

    public string GetTopSpenderCustomerName() =>
        OrderAnalytics.GetTopSpenderCustomerName(_repo.GetAll(), _discountPolicy);
}