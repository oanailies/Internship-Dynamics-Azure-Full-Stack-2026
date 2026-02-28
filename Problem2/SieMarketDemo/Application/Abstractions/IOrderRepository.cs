using System.Collections.Generic;
using SieMarket.Domain.Orders;

namespace SieMarket.Application.Abstractions;

public interface IOrderRepository
{
    void Add(Order order);
    IReadOnlyList<Order> GetAll();
}