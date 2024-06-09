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
        var totalRequiredQuantities = new Dictionary<Guid, int>();

        foreach (var item in Items)
        {
            if (totalRequiredQuantities.ContainsKey(item.Product.Id))
            {
                totalRequiredQuantities[item.Product.Id] += item.Quantity;
            }
            else
            {
                totalRequiredQuantities[item.Product.Id] = item.Quantity;
            }
        }

        bool canFulfillAtLeastOne = false;

        foreach (var required in totalRequiredQuantities)
        {
            var totalAvailableQuantity = skus
                .Where(s => s.Product.Id == required.Key)
                .Sum(s => s.GetAvailableQuantity());

            if (totalAvailableQuantity >= required.Value)
            {
                canFulfillAtLeastOne = true;
            }

            if (CompleteDeliveryRequired && totalAvailableQuantity < required.Value)
            {
                var product = Items.First(item => item.Product.Id == required.Key).Product;
                Console.WriteLine(
                    $"Cannot fulfill order for product {product.Name} due to insufficient stock. Available: {totalAvailableQuantity}, Required: {required.Value}, Complete Delivery Required: {CompleteDeliveryRequired}");
                return false;
            }
        }

        return canFulfillAtLeastOne;
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
