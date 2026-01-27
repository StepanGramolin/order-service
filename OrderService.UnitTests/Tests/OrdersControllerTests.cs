using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

using OrderService.WebApi.Controllers;
using OrderService.WebApi.UseCases.Commands;

namespace OrderService.Tests;

[TestFixture]
public class OrdersControllerTests
{
    private Mock<IMediator> _mediatorMock;
    private OrdersController _controller;

    [SetUp]
    public void Setup()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new OrdersController(_mediatorMock.Object);
    }

    [Test]
    public async Task Create_ValidCommand_ShouldReturnOkWithOrderId()
    {
        // Arrange
        var command = new CreateOrderCommand(1, 1, "a@b.com", 100, "123");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(123L); // Имитируем возврат ID

        // Act
        var result = await _controller.Create(command, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        (result.Result as OkObjectResult)!.Value.Should().Be(123L);
        _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Get_ExistingOrder_ShouldReturnOkWithOrderResponse()
    {
        // Arrange
        var orderResponse = new GetOrderResponse(1, 1, "a@b.com", 100, "123");
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetOrderQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(orderResponse);

        // Act
        var result = await _controller.Get(1, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        (result.Result as OkObjectResult)!.Value.Should().Be(orderResponse);
        _mediatorMock.Verify(m => m.Send(It.Is<GetOrderQuery>(q => q.OrderId == 1), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Get_NonExistingOrder_ShouldReturnNotFound()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetOrderQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((GetOrderResponse?)null);

        // Act
        var result = await _controller.Get(999, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<GetOrderQuery>(q => q.OrderId == 999), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Delete_ExistingOrder_ShouldReturnNoContent()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteOrderCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<DeleteOrderCommand>(c => c.OrderId == 1), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Delete_NonExistingOrder_ShouldReturnNotFound()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteOrderCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(999, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<DeleteOrderCommand>(c => c.OrderId == 999), It.IsAny<CancellationToken>()), Times.Once);
    }
}