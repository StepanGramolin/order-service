using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using FluentAssertions;

using OrderService.DataAccess.Postgres.AppDbContext;
using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.UseCases;
using OrderService.WebApi.UseCases.Commands;

namespace OrderService.Tests.Tests;

[TestFixture]
public class DeleteOrderHandlerTests
{
    private OrdersDbContext _dbContext;
    private DeleteOrderHandler _handler;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new OrdersDbContext(options);
        _handler = new DeleteOrderHandler(_dbContext);
    }

    [TearDown]
    public void TearDown() => _dbContext.Dispose();

    [Test]
    public async Task Handle_ExistingOrderId_ShouldRemoveOrderAndReturnTrue()
    {
        var order = new Order { OrderId = 1, ProductId = 10, Amount = 1, Price = 100, EmailClient = "test@mail.com", PhoneNumber = "+71234" };
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var command = new DeleteOrderCommand(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        var orderInDb = await _dbContext.Orders.FindAsync(1L);
        orderInDb.Should().BeNull(); // Проверяем, что заказа нет в базе
    }

    [Test]
    public async Task Handle_NonExistingOrderId_ShouldReturnFalse()
    {
        var command = new DeleteOrderCommand(999);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}