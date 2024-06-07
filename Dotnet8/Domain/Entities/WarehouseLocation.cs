namespace Dotnet8.Domain.Entities;

public class WarehouseLocation(Guid id, bool isLocked)
{
    public Guid Id { get; } = id;
    public bool IsLocked { get; } = isLocked;
}
