using Dotnet8.Domain.Aggregates;
using Dotnet8.Domain.Entities;
using Dotnet8.Domain.Enums;
using Dotnet8.Domain.ValueObjects;

namespace Dotnet8.Infrastructure;

public static class SimulationDb
{
    private static List<Product> Products { get; set; } = new();
    private static List<WarehouseLocation> WarehouseLocations { get; set; } = new();
    public static List<Sku> Skus { get; private set; } = new();
    public static List<Order> Orders { get; private set; } = new();
    public static List<Allocation> Allocations { get; private set; } = new();

    public static void Seed()
    {
        Products = new List<Product>
        {
            new(Guid.NewGuid(), "205/55 R16 All-Season"),
            new(Guid.NewGuid(), "225/50 R17 All-Season"),
            new(Guid.NewGuid(), "195/65 R15 All-Season")
        };

        WarehouseLocations = new List<WarehouseLocation>
        {
            new(Guid.NewGuid(), false),
            new(Guid.NewGuid(), true),
            new(Guid.NewGuid(), false)
        };

        Skus = new List<Sku>
        {
            new(Guid.NewGuid(), Products[0], WarehouseLocations[0], 30),
            new(Guid.NewGuid(), Products[1], WarehouseLocations[1], 20),
            new(Guid.NewGuid(), Products[2], WarehouseLocations[2], 50)
        };

        Orders = new List<Order>
        {
            new(Guid.NewGuid(), true, Priority.High,
                new List<LineItem> { new(Guid.NewGuid(), Products[0], 10) }, DateTime.Now.AddHours(-4)),

            new(Guid.NewGuid(), false, Priority.Normal, new List<LineItem>
            {
                new(Guid.NewGuid(), Products[1], 5), new(Guid.NewGuid(), Products[2], 15)
            }, DateTime.Now.AddHours(-3)),

            new(Guid.NewGuid(), false, Priority.Low,
                new List<LineItem> { new LineItem(Guid.NewGuid(), Products[2], 10) }, DateTime.Now.AddHours(-1)),

            new(Guid.NewGuid(), true, Priority.Normal,
                new List<LineItem> { new LineItem(Guid.NewGuid(), Products[0], 40) }, DateTime.Now.AddHours(-3)),

            new(Guid.NewGuid(), false, Priority.High,
                new List<LineItem> { new LineItem(Guid.NewGuid(), Products[2], 30) }, DateTime.Now.AddHours(-2))
        };

        Allocations = new List<Allocation>();
    }
}