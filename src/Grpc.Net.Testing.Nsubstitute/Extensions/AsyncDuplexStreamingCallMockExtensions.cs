using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Testing.Nsubstitute.Shared;
using NSubstitute;
using NSubstitute.Core;

namespace Grpc.Net.Testing.Nsubstitute.Extensions;

public static class AsyncDuplexStreamingCallMockExtensions
{
    public static ConfiguredCall Returns<TRequest, TResponse>(
        this AsyncDuplexStreamingCall<TRequest, TResponse> value,
        params TResponse[] returnThis)
        => Returns(value, () => returnThis);

    public static ConfiguredCall Returns<TRequest, TResponse>(
        this AsyncDuplexStreamingCall<TRequest, TResponse> value,
        Func<IEnumerable<TResponse>> func)
        => Returns(value, _ => func());

    public static ConfiguredCall Returns<TRequest, TResponse>(
        this AsyncDuplexStreamingCall<TRequest, TResponse> value,
        Func<IEnumerable<TRequest>, IEnumerable<TResponse>> func)
        => value.Returns(
            _ =>
            {
                var requestStream = new WhenStreamWriter<TRequest>();
                var handler = () => func(requestStream.ReadAll());
                var responseStream = new WhenStreamReader<TResponse>(handler);

                var fakeCall = new AsyncDuplexStreamingCall<TRequest, TResponse>(
                    requestStream,
                    responseStream,
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { });

                return fakeCall;
            });
}
