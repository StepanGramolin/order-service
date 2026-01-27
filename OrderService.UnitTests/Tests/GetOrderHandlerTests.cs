using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using FluentAssertions;

using OrderService.DataAccess.Postgres.AppDbContext;
using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.Mappers;
using OrderService.WebApi.UseCases;
using OrderService.WebApi.UseCases.Commands;

namespace OrderService.UnitTests.Tests;

[TestFixture]
public class GetOrderHandlerTests
{
    private OrdersDbContext _dbContext;
    private OrderMapper _mapper;
    private GetOrderHandler _handler;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new OrdersDbContext(options);
        _mapper = new OrderMapper();
        _handler = new GetOrderHandler(_dbContext, _mapper);
    }

    [TearDown]
    public void TearDown() => _dbContext.Dispose();

    [Test]
    public async Task Handle_ExistingOrderId_ShouldReturnOrderResponse()
    {
        var order = new Order { OrderId = 1, ProductId = 10, Amount = 1, Price = 100, EmailClient = "test@mail.com", PhoneNumber = "+71234" };
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var query = new GetOrderQuery(1);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.ProductId.Should().Be(order.ProductId);
        result.Price.Should().Be(order.Price);
    }

    [Test]
    public async Task Handle_NonExistingOrderId_ShouldReturnNull()
    {
        var query = new GetOrderQuery(999); // ID, которого нет в базе

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }
}