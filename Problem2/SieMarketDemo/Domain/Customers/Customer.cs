using System;

namespace SieMarket.Domain.Customers;

public sealed class Customer
{
    public Customer(CustomerId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Customer name cannot be empty.", nameof(name));

        Id = id;
        Name = name.Trim();
    }

    public CustomerId Id { get; }
    public string Name { get; }
}