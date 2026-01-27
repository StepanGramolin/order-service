using Moq;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using OrderService.WebApi.UseCases.Commands;
using OrderService.WebApi.UseCases;
using OrderService.WebApi.Infrastructure;
using OrderService.WebApi.Mappers;
using OrderService.DataAccess.Postgres.AppDbContext;

namespace OrderService.Tests.UseCases;

[TestFixture]
public class CreateOrderHandlerTests
{
    private OrdersDbContext _dbContext;
    private Mock<IPaymentsClient> _paymentsClientMock;
    private Mock<IKafkaProducer> _kafkaProducerMock;
    private CreateOrderHandler _handler;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _dbContext = new OrdersDbContext(options);

        _paymentsClientMock = new Mock<IPaymentsClient>();
        _kafkaProducerMock = new Mock<IKafkaProducer>();

        _handler = new CreateOrderHandler(
            _dbContext,
            new OrderMapper(),
            _paymentsClientMock.Object,
            _kafkaProducerMock.Object);
    }

    [Test]
    public async Task Handle_ShouldSaveOrderToDatabase()
    {
        var command = new CreateOrderCommand(1, 1, "test@test.com", 100, "+7999");
        var resultId = await _handler.Handle(command, CancellationToken.None);

        resultId.Should().BeGreaterThan(0);
        var order = await _dbContext.Orders.FindAsync(resultId);
        order.Should().NotBeNull();
    }
}
