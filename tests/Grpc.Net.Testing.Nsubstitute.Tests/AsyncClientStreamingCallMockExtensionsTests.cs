using System;
using System.Linq;
using System.Threading;
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

public class AsyncClientStreamingCallMockExtensionsTests
{
    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse(TestResponse expectedResponse)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleClientStream(Arg.Any<Metadata>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var call = grpcMock.SimpleClientStream();

        await call.RequestStream.CompleteAsync();

        var message = await call;

        // Assert
        message.Should().BeEquivalentTo(expectedResponse);
    }

    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse_WithRequests(TestRequest[] requests, TestResponse expectedResponses)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleClientStream(Arg.Any<Metadata>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponses);

        // Act
        var call = grpcMock.SimpleClientStream();

        await call.RequestStream.WriteAllAsync(requests, complete: true);

        var message = await call;

        // Assert
        message.Should().BeEquivalentTo(expectedResponses);
    }

    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse_ByLambda(TestResponse expectedResponses)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleClientStream(Arg.Any<Metadata>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns(() => expectedResponses);

        // Act
        var call = grpcMock.SimpleClientStream();

        await call.RequestStream.CompleteAsync();

        var message = await call;

        // Assert
        message.Should().BeEquivalentTo(expectedResponses);
    }

    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse_ByLambda_WithRequests(TestRequest[] requests, TestResponse expectedResponses)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleClientStream(Arg.Any<Metadata>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns(() => expectedResponses);

        // Act
        var call = grpcMock.SimpleClientStream();

        await call.RequestStream.WriteAllAsync(requests, complete: true);

        var message = await call;

        // Assert
        message.Should().BeEquivalentTo(expectedResponses);
    }

    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse_ByLambdaWithRequests(TestRequest[] requests)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleClientStream(Arg.Any<Metadata>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns(rs => new TestResponse { Val = rs.Sum(r => r.Val) });

        // Act
        var call = grpcMock.SimpleClientStream();

        await call.RequestStream.WriteAllAsync(requests, complete: true);

        var message = await call;

        // Assert
        var expectedValue = requests.Sum(r => r.Val);

        message.Val.Should().Be(expectedValue);
    }

    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse_ByLambdaWithRequestsAndNotStandardWayToAwait(TestRequest[] requests)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleClientStream(Arg.Any<Metadata>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns(rs => new TestResponse { Val = rs.Sum(r => r.Val) });

        // Act
        var call = grpcMock.SimpleClientStream();

        var requestTask = call.RequestStream.WriteAllAsync(requests, complete: true);

        var message = await call;

        await requestTask;

        // Assert
        var expectedValue = requests.Sum(r => r.Val);

        message.Val.Should().Be(expectedValue);
    }
}
