using Electrify.Protos;
using Electrify.Server.Services;
using Electrify.Server.Services.Abstraction;
using FluentAssertions;
using Grpc.Core;
using NSubstitute;

namespace Electrify.Server.UnitTests.Services;

public class ClientBillServiceTests
{
    private readonly IReadingService _readingService = Substitute.For<IReadingService>();
    private readonly ClientBillService _clientBillService;
    
    public ClientBillServiceTests()
    {
        _clientBillService = new ClientBillService(_readingService);
    }
    
    [Fact]
    public async Task GetClientBill_Should_Throw_InvalidArgument_When_ClientId_Not_Guid()
    {
        // Arrange
        var request = new GetClientBillRequest
        {
            ClientId = "NotAGuid",
            Date = "01/01/2022"
        };
        
        var context = Substitute.For<ServerCallContext>();
        
        // Act
        Func<Task> act = async () => await _clientBillService.GetClientBill(request, context);
        
        // Assert
        await act.Should().ThrowAsync<RpcException>();
    }
    
    [Fact]
    public async Task GetClientBill_Should_Throw_InvalidArgument_When_Date_Not_DateOnly()
    {
        // Arrange
        var request = new GetClientBillRequest
        {
            ClientId = Guid.NewGuid().ToString(),
            Date = "NotADate"
        };
        
        var context = Substitute.For<ServerCallContext>();
        
        // Act
        Func<Task> act = async () => await _clientBillService.GetClientBill(request, context);
        
        // Assert
        await act.Should().ThrowAsync<RpcException>();
    }
    
    [Fact]
    public async Task GetClientBill_Should_Return_PayableAmount()
    {
        // Arrange
        DateOnly date = new DateOnly(2022, 8, 1);
        
        var request = new GetClientBillRequest
        {
            ClientId = Guid.NewGuid().ToString(),
            Date = date.ToString("dd/MM/yyyy")
        };
        
        _readingService.GetReadingsForDay(Guid.Parse(request.ClientId), date).Returns(100.0);
        
        var context = Substitute.For<ServerCallContext>();
        
        // Act
        var result = await _clientBillService.GetClientBill(request, context);
        
        // Assert
        result.PayableAmount.Should().Be(100.0);
    }
}