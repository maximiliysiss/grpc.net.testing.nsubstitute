using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Grpc.Core;
using Grpc.Net.Testing.Nsubstitute.Extensions;
using Grpc.Net.Testing.Nsubstitute.Tests.Proto;
using NSubstitute;
using Xunit;

namespace Grpc.Net.Testing.Nsubstitute.Tests;

public class AsyncUnaryCallExtensionsTests
{
    [Theory, AutoData]
    public void Simple_ShouldReturnResponse(TestResponse expectedResponse)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .Simple(Arg.Any<TestRequest>(), null, null, default)
            .Returns(expectedResponse);

        // Act
        var response = grpcMock.Simple(new TestRequest());

        // Assert
        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Theory, AutoData]
    public async Task SimpleAsync_ShouldReturnExpectedResponse(TestResponse expectedResponse)
    {
        // Arrange
        var grpcClient = Substitute.For<TestService.TestServiceClient>();
        grpcClient
            .SimpleAsync(Arg.Any<TestRequest>(), Arg.Any<Metadata>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        var testRequest = new TestRequest();

        // Act
        var response = await grpcClient.SimpleAsync(testRequest);

        // Assert
        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Theory, AutoData]
    public async Task SimpleAsync_ShouldReturnExpectedResponse_ByLambda(TestResponse expectedResponse)
    {
        // Arrange
        var grpcClient = Substitute.For<TestService.TestServiceClient>();
        grpcClient
            .SimpleAsync(Arg.Any<TestRequest>(), Arg.Any<Metadata>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns(() => expectedResponse);

        var testRequest = new TestRequest();

        // Act
        var response = await grpcClient.SimpleAsync(testRequest);

        // Assert
        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Theory, AutoData]
    public async Task SimpleAsync_ShouldReturnExpectedResponse_ByLambdaAndRequest(TestRequest request)
    {
        // Arrange
        var grpcClient = Substitute.For<TestService.TestServiceClient>();
        grpcClient
            .SimpleAsync(Arg.Any<TestRequest>(), Arg.Any<Metadata>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns<TestRequest, TestResponse>(r => new TestResponse { Val = r.Val });

        // Act
        var response = await grpcClient.SimpleAsync(request);

        // Assert
        response.Val.Should().Be(request.Val);
    }
}
