namespace Dotnet8.Domain.Entities;

public class Allocation(Guid orderId, Guid lineItemId, Guid skuId, int quantity)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; } = orderId;
    public Guid LineItemId { get; set; } = lineItemId;
    public Guid SkuId { get; set; } = skuId;
    public int Quantity { get; set; } = quantity;
}