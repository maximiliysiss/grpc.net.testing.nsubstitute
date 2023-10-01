using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Testing.Nsubstitute.Shared;
using NSubstitute;
using NSubstitute.Core;

namespace Grpc.Net.Testing.Nsubstitute.Extensions;

public static class AsyncClientStreamingCallMockExtensions
{
    public static ConfiguredCall Returns<TRequest, TResponse>(
        this AsyncClientStreamingCall<TRequest, TResponse> value,
        TResponse returnThis)
        => Returns(value, () => returnThis);

    public static ConfiguredCall Returns<TRequest, TResponse>(
        this AsyncClientStreamingCall<TRequest, TResponse> value,
        Func<TResponse> func)
        => Returns(value, _ => func());

    public static ConfiguredCall Returns<TRequest, TResponse>(
        this AsyncClientStreamingCall<TRequest, TResponse> value,
        Func<IEnumerable<TRequest>, TResponse> func)
        => value.Returns(
            _ =>
            {
                var requestStream = new WhenStreamWriter<TRequest>();
                var stream = requestStream.ReadAll();
                var responseTask = Task.Run(() => func(stream));

                var fakeCall = new AsyncClientStreamingCall<TRequest, TResponse>(
                    requestStream,
                    responseTask,
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { });

                return fakeCall;
            });
}
