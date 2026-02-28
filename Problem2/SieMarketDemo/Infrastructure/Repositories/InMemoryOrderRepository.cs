using System.Collections.Generic;
using SieMarket.Application.Abstractions;
using SieMarket.Domain.Orders;

namespace SieMarket.Infrastructure.Repositories;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = new();

    public void Add(Order order) => _orders.Add(order);

    public IReadOnlyList<Order> GetAll() => _orders;
}