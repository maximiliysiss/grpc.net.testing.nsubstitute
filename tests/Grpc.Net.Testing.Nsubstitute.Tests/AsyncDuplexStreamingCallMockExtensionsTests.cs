using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Grpc.Core;
using Grpc.Core.Utils;
using Grpc.Net.Testing.Nsubstitute.Extensions;
using Grpc.Net.Testing.Nsubstitute.Tests.Proto;
using NSubstitute;
using Xunit;

namespace Grpc.Net.Testing.Nsubstitute.Tests;

public class AsyncDuplexStreamingCallMockExtensionsTests
{
    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse(TestResponse[] expectedResponses)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleClientServerStream(null, null, default)
            .Returns(expectedResponses);

        // Act
        var call = grpcMock.SimpleClientServerStream();

        await call.RequestStream.CompleteAsync();

        var messages = await call.ResponseStream
            .ReadAllAsync()
            .ToArrayAsync();

        // Assert
        messages.Should().BeEquivalentTo(expectedResponses);
    }

    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse_WithRequest(TestRequest[] requests, TestResponse[] expectedResponses)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleClientServerStream(null, null, default)
            .Returns(expectedResponses);

        // Act
        var call = grpcMock.SimpleClientServerStream();

        await call.RequestStream.WriteAllAsync(requests, complete: true);

        var messages = await call.ResponseStream.ReadAllAsync().ToArrayAsync();

        // Assert
        messages.Should().BeEquivalentTo(expectedResponses);
    }

    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse_ByLambda(TestResponse[] expectedResponses)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleClientServerStream(null, null, default)
            .Returns(() => expectedResponses);

        // Act
        var call = grpcMock.SimpleClientServerStream();

        await call.RequestStream.CompleteAsync();

        var messages = await call.ResponseStream.ReadAllAsync().ToArrayAsync();

        // Assert
        messages.Should().BeEquivalentTo(expectedResponses);
    }

    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse_ByLambda_WithRequests(TestRequest[] requests, TestResponse[] expectedResponses)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleClientServerStream(null, null, default)
            .Returns(() => expectedResponses);

        // Act
        var call = grpcMock.SimpleClientServerStream();

        await call.RequestStream.WriteAllAsync(requests, complete: true);

        var messages = await call.ResponseStream.ReadAllAsync().ToArrayAsync();

        // Assert
        messages.Should().BeEquivalentTo(expectedResponses);
    }

    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse_ByLambdaWithRequest(TestRequest[] requests)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleClientServerStream(null, null, default)
            .Returns(rs => rs.Select(r => new TestResponse { Val = r.Val }));

        // Act
        var call = grpcMock.SimpleClientServerStream();

        await call.RequestStream.WriteAllAsync(requests, complete: true);

        var messages = await call.ResponseStream.ReadAllAsync().ToArrayAsync();

        // Assert
        for (var i = 0; i < messages.Length; i++)
        {
            var request = requests[i];
            var response = messages[i];

            response.Val.Should().Be(request.Val);
        }
    }

    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse_ByLambdaWithRequestAndNotStandardWayToAwait(TestRequest[] requests)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleClientServerStream(null, null, default)
            .Returns(rs => rs.Select(r => new TestResponse { Val = r.Val }));

        // Act
        var call = grpcMock.SimpleClientServerStream();

        var requestTask = call.RequestStream.WriteAllAsync(requests, complete: true);

        var messages = await call.ResponseStream.ReadAllAsync().ToArrayAsync();

        await requestTask;

        // Assert
        for (var i = 0; i < messages.Length; i++)
        {
            var request = requests[i];
            var response = messages[i];

            response.Val.Should().Be(request.Val);
        }
    }

    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse_ByLambdaWithRequestAndAccumulate(TestRequest[] requests)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleClientServerStream(null, null, default)
            .Returns(rs => new[] { new TestResponse { Val = rs.Sum(r => r.Val) } });

        // Act
        var call = grpcMock.SimpleClientServerStream();

        var requestTask = call.RequestStream.WriteAllAsync(requests, complete: true);

        var messages = await call.ResponseStream.ReadAllAsync().ToArrayAsync();

        await requestTask;

        // Assert
        var expectedValue = requests.Sum(r => r.Val);

        messages
            .Should().ContainSingle()
            .Which.Val.Should().Be(expectedValue);
    }
}
