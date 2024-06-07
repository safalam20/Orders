using Dotnet8.Infrastructure;

namespace Dotnet8.Application;

public class StockAllocationService
{
    /// <summary>
    /// Allocates stock to customer orders based on their priority and the available stock.
    /// </summary>
    /// <remarks>
    /// This method processes each order in order of their priority, from highest to lowest 
    /// For each order, it checks if there is enough stock available to fulfill the order.
    /// If there is enough stock, it allocates the required quantities to the order.
    /// </remarks>
    public void AllocateStock()
    {
        var orders = SimulationDb.Orders
            .OrderByDescending(o => o.Priority)
            .ThenBy(o => o.OrderDate)
            .ToList();

        Console.WriteLine("Orders have been sorted by priority and date.");
        Console.WriteLine("Sorted Order IDs: " + string.Join(", ", orders.Select(o => o.Id)));

        foreach (var order in orders)
        {
            Console.WriteLine($"Now handling order with ID: {order.Id}");

            var skus = SimulationDb.Skus;

            if (order.CanFulfillOrder(skus))
            {
                order.FulfillOrder(skus);
            }
        }
    }

    /// <summary>
    /// Cancels an entire order and deallocates the stock.
    /// </summary>
    /// <param name="orderId">The ID of the order to cancel.</param>
    public void CancelOrder(Guid orderId)
    {
        var order = SimulationDb.Orders.Single(o => o.Id == orderId);
        var lineItemIds = order.Items.Select(item => item.Id).ToList();
        foreach (var lineItemId in lineItemIds)
        {
            CancelLineItem(orderId, lineItemId);
        }

        SimulationDb.Orders.Remove(order);
        Console.WriteLine($"Order {orderId} has been canceled and stock reallocated.");
    }

    /// <summary>
    /// Cancels a specific line item within an order and deallocates the stock.
    /// </summary>
    /// <param name="orderId">The ID of the order containing the line item.</param>
    /// <param name="lineItemId">The ID of the line item to cancel.</param>
    public void CancelLineItem(Guid orderId, Guid lineItemId)
    {
        var allocations = SimulationDb.Allocations
            .Where(a => a.OrderId == orderId && a.LineItemId == lineItemId)
            .ToList();

        foreach (var allocation in allocations)
        {
            var sku = SimulationDb.Skus.Single(s => s.Id == allocation.SkuId);
            sku.Allocations.Remove(allocation);
            SimulationDb.Allocations.Remove(allocation);
            Console.WriteLine(
                $"Deallocated {allocation.Quantity} of SKU {sku.Id} for order {orderId}, line item {lineItemId}");
        }

        var order = SimulationDb.Orders.Single(o => o.Id == orderId);

        var lineItem = order.Items.Single(li => li.Id == lineItemId);
        order.Items.Remove(lineItem);
        Console.WriteLine(
            $"Line item {lineItemId} from order {orderId} has been canceled and stock reallocated.");
    }
}