using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Grpc.Core;
using Grpc.Net.Testing.Nsubstitute.Extensions;
using Grpc.Net.Testing.Nsubstitute.Tests.Proto;
using NSubstitute;
using Xunit;

namespace Grpc.Net.Testing.Nsubstitute.Tests;

public class AsyncServerStreamingCallMockExtensionsTests
{
    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse(TestResponse[] expectedResponses)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleServerStream(Arg.Any<TestRequest>(), null, null, default)
            .Returns(expectedResponses);

        // Act
        var call = grpcMock.SimpleServerStream(new TestRequest());
        var messages = await call.ResponseStream.ReadAllAsync().ToArrayAsync();

        // Assert
        messages.Should().BeEquivalentTo(expectedResponses);
    }

    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse_WhenCalledByLambda(TestResponse[] expectedResponses)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleServerStream(Arg.Any<TestRequest>(), null, null, default)
            .Returns(() => expectedResponses);

        // Act
        var call = grpcMock.SimpleServerStream(new TestRequest());
        var messages = await call.ResponseStream.ReadAllAsync().ToArrayAsync();

        // Assert
        messages.Should().BeEquivalentTo(expectedResponses);
    }

    [Theory, AutoData]
    public async Task SimpleServer_ShouldReturnResponse_WhenCalledByLambdaWithRequest(TestRequest request)
    {
        // Arrange
        var grpcMock = Substitute.For<TestService.TestServiceClient>();
        grpcMock
            .SimpleServerStream(Arg.Any<TestRequest>(), null, null, default)
            .Returns<TestRequest, TestResponse>(r => Enumerable.Repeat(new TestResponse { Val = r.Val }, 2));

        // Act
        var call = grpcMock.SimpleServerStream(request);
        var messages = await call.ResponseStream.ReadAllAsync().ToArrayAsync();

        // Assert
        messages.Should().AllSatisfy(response => response.Val.Should().Be(request.Val));
    }
}
