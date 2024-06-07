namespace Dotnet8.Domain.Entities;

public class Product(Guid id, string name)
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
}
