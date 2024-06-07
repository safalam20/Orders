using Dotnet8.Domain.Entities;
using Dotnet8.Domain.Enums;
using Dotnet8.Domain.ValueObjects;
using Dotnet8.Infrastructure;

namespace Dotnet8.Domain.Aggregates;

public class Order(Guid id, bool completeDeliveryRequired, Priority priority, List<LineItem> items, DateTime orderDate)
{
    public Guid Id { get; } = id;
    public List<LineItem> Items { get; } = items;
    private bool CompleteDeliveryRequired { get; } = completeDeliveryRequired;
    public Priority Priority { get; } = priority;
    public DateTime OrderDate { get; } = orderDate;

    public bool CanFulfillOrder(List<Sku> skus)
    {
        foreach (var item in Items)
        {
            var totalAvailableQuantity = skus
                .Where(s => s.Product.Id == item.Product.Id)
                .Sum(s => s.GetAvailableQuantity());

            if (CompleteDeliveryRequired && totalAvailableQuantity < item.Quantity)
            {
                Console.WriteLine(
                    $"Cannot fulfill order for product {item.Product.Name} due to insufficient stock. Available: {totalAvailableQuantity}, Required: {item.Quantity}, Complete Delivery Required: {CompleteDeliveryRequired}");
                return false;
            }
        }

        return true;
    }

    public void FulfillOrder(List<Sku> skus)
    {
        foreach (var item in Items)
        {
            var remainingQuantity = item.Quantity;
            var initialQuantity = item.Quantity;
            var allocationsSummary = new List<string>();

            foreach (var sku in skus.Where(s => s.Product.Id == item.Product.Id && s.GetAvailableQuantity() > 0))
            {
                if (remainingQuantity <= 0) break;
                var allocatedQuantity = Math.Min(sku.GetAvailableQuantity(), remainingQuantity);
                remainingQuantity -= allocatedQuantity;

                var allocation = new Allocation(Id, item.Id, sku.Id, allocatedQuantity);
                SimulationDb.Allocations.Add(allocation);
                sku.Allocations.Add(allocation);

                allocationsSummary.Add($"SKU {sku.Id} provided {allocatedQuantity}");
            }

            if (remainingQuantity > 0)
            {
                Console.WriteLine(
                    $"Failed to fully allocate stock for product {item.Product.Name}. Required: {initialQuantity}, Allocated: {initialQuantity - remainingQuantity}, Remaining: {remainingQuantity}. Details: {string.Join(", ", allocationsSummary)}");
            }
            else
            {
                Console.WriteLine(
                    $"Successfully allocated stock for product {item.Product.Name}. Required: {initialQuantity}, Allocated: {initialQuantity}. Details: {string.Join(", ", allocationsSummary)}");
            }
        }
    }
}