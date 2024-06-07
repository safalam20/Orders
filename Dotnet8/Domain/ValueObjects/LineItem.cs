using Dotnet8.Domain.Entities;

namespace Dotnet8.Domain.ValueObjects;

public class LineItem(Guid Id, Product product, int quantity)
{
    public Guid Id { get; set; } = Id;
    public Product Product { get; } = product;
    public int Quantity { get; } = quantity;
}
