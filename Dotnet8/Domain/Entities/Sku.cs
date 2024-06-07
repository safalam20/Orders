namespace Dotnet8.Domain.Entities;

public class Sku(Guid id, Product product, WarehouseLocation warehouseLocation, int quantity)
{
    public Guid Id { get; } = id;
    public Product Product { get; } = product;
    private WarehouseLocation WarehouseLocation { get; } = warehouseLocation;
    private int Quantity { get; set; } = quantity;
    public List<Allocation> Allocations { get; set; } = new();

    public int GetAvailableQuantity()
    {
        if (WarehouseLocation.IsLocked)
        {
            return 0;
        }

        int allocatedQuantity = Allocations.Sum(a => a.Quantity);
        return Quantity - allocatedQuantity;
    }
}