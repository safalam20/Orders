using Dotnet8.Application;
using Dotnet8.Infrastructure;

Console.WriteLine("Hello, World!");

SimulationDb.Seed();

var allocationService = new StockAllocationService();

allocationService.AllocateStock();

allocationService.CancelOrder(SimulationDb.Orders[0].Id);


