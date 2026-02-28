using System;
using System.Collections.Generic;
using System.Linq;
using SieMarket.Domain.Customers;
using SieMarket.Domain.Discounts;
using SieMarket.Domain.Orders;

var discountPolicy = new ThresholdDiscountPolicy(threshold: 500m, discountRate: 0.10m);

// Customers
var alice = new Customer(CustomerId.New(), "Alice");
var bob   = new Customer(CustomerId.New(), "Bob");
var carla = new Customer(CustomerId.New(), "Carla");
var dan   = new Customer(CustomerId.New(), "Dan");
var eva   = new Customer(CustomerId.New(), "Eva");

// Orders 
var orders = new List<Order>
{
    // Alice
    CreateOrder(alice,
        new OrderItem("Gaming Laptop", 1, 700.00m),      // discount
        new OrderItem("Mouse", 1, 29.99m)),

    CreateOrder(alice,
        new OrderItem("Smartphone", 1, 499.00m),         // subtotal 559 => discount
        new OrderItem("Wireless Earbuds", 1, 60.00m),
        new OrderItem("HDMI Cable", 2, 12.50m)),

    // Bob
    CreateOrder(bob,
        new OrderItem("Monitor 27\"", 2, 250.00m)),      // EXACT 500 => NO discount

    CreateOrder(bob,
        new OrderItem("Keyboard", 1, 119.99m),
        new OrderItem("mouse", 2, 25.00m),              
        new OrderItem("USB-C Cable", 3, 9.99m)),

    CreateOrder(bob,
        new OrderItem("External SSD 1TB", 1, 129.95m),
        new OrderItem("Webcam", 1, 79.50m),
        new OrderItem("Headset", 1, 89.00m)),          
    // Carla
    CreateOrder(carla,
        new OrderItem("TV 55\"", 1, 899.99m),            
        new OrderItem("HDMI Cable", 1, 12.50m)),

    CreateOrder(carla,
        new OrderItem("HDMI Cable", 3, 12.50m),
        new OrderItem("Power Strip", 1, 34.90m),
        new OrderItem("Soundbar", 1, 459.00m)),          
    // Dan (many orders, repeated items)
    CreateOrder(dan,
        new OrderItem("USB-C Cable", 5, 9.99m),
        new OrderItem("Charger 65W", 1, 39.90m),
        new OrderItem("Mouse", 1, 19.99m)),

    CreateOrder(dan,
        new OrderItem("Office Monitor", 1, 219.00m),
        new OrderItem("Docking Station", 1, 299.00m),    
        new OrderItem("HDMI Cable", 4, 12.50m)),

    CreateOrder(dan,
        new OrderItem("Keyboard", 1, 89.99m),
        new OrderItem("USB-C Cable", 2, 9.99m),
        new OrderItem("mouse", 1, 24.99m)),

    // Eva 
    CreateOrder(eva,
        new OrderItem("USB-C Cable", 12, 8.99m),
        new OrderItem("HDMI Cable", 10, 10.99m),
        new OrderItem("Mouse", 3, 18.50m)),

    CreateOrder(eva,
        new OrderItem("Power Strip", 4, 22.00m),
        new OrderItem("Keyboard", 2, 99.00m),
        new OrderItem("Webcam", 2, 75.00m))            
};

Console.WriteLine("=== SieMarket Demo (Discount > 500 EUR => 10%) ===\n");

// Print each order details
int idx = 1;
foreach (var order in orders)
{
    var subtotal = order.GetSubtotal();
    var finalPrice = order.CalculateFinalPrice(discountPolicy);
    var discountAmount = subtotal - finalPrice;
    var discountApplied = discountAmount > 0.0001m;

    Console.WriteLine($"Order #{idx++}");
    Console.WriteLine($"Customer: {order.Customer.Name}");
    Console.WriteLine("Items:");
    foreach (var item in order.Items)
        Console.WriteLine($"  - {item.ProductName} | qty: {item.Quantity} | unit: {item.UnitPrice:0.00} | line: {item.TotalPrice:0.00}");

    Console.WriteLine($"Subtotal: {subtotal:0.00} EUR");
    Console.WriteLine(discountApplied
        ? $"Discount: -{discountAmount:0.00} EUR (applied)"
        : $"Discount: 0.00 EUR (not applied)");

    Console.WriteLine($"Final:    {finalPrice:0.00} EUR");
    Console.WriteLine(new string('-', 52));
}

// 2.3 totals + top spender
Console.WriteLine("\n=== Total spent per customer (FINAL prices) ===");

var totals = orders
    .GroupBy(o => o.Customer.Name)
    .Select(g => new
    {
        Customer = g.Key,
        OrdersCount = g.Count(),
        TotalSpent = g.Sum(o => o.CalculateFinalPrice(discountPolicy))
    })
    .OrderByDescending(x => x.TotalSpent)
    .ToList();

foreach (var t in totals)
    Console.WriteLine($"{t.Customer,-8} | orders: {t.OrdersCount,2} | total: {t.TotalSpent:0.00} EUR");

var topSpender = OrderAnalytics.GetTopSpenderCustomerName(orders, discountPolicy);
Console.WriteLine($"\nTop spender: {topSpender}");

// 2.4 popular products + qty sold
Console.WriteLine("\n=== Popular products (total quantity sold) ===");

var popular = OrderAnalytics.GetPopularProductsWithTotalQuantitySold(orders);

foreach (var p in popular)
    Console.WriteLine($"{p.ProductName,-18} | qty sold: {p.TotalQuantitySold}");

Console.WriteLine("\n=== Top 5 popular products ===");
foreach (var p in popular.Take(5))
    Console.WriteLine($"{p.ProductName,-18} | qty sold: {p.TotalQuantitySold}");

var totalQtyFromProducts = popular.Sum(x => x.TotalQuantitySold);
var totalQtyFromOrders = orders.SelectMany(o => o.Items).Sum(i => i.Quantity);

Console.WriteLine($"\nSanity check (should match): products sum = {totalQtyFromProducts}, orders sum = {totalQtyFromOrders}");


// -------- helpers --------
static Order CreateOrder(Customer customer, params OrderItem[] items)
{
    var order = new Order(OrderId.New(), customer);
    foreach (var item in items) order.AddItem(item);
    return order;
}