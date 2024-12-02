using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using NSubstitute;
using NSubstitute.Core;

namespace Grpc.Net.Testing.Nsubstitute.Extensions;

public static class AsyncServerStreamingCallMockExtensions
{
    public static ConfiguredCall Returns<TResponse>(
        this AsyncServerStreamingCall<TResponse> value,
        params TResponse[] returnThis)
        => Returns(value, () => returnThis);

    public static ConfiguredCall Returns<TResponse>(
        this AsyncServerStreamingCall<TResponse> value,
        Func<IEnumerable<TResponse>> func)
        => Returns<object, TResponse>(value, _ => func());

    public static ConfiguredCall Returns<TRequest, TResponse>(
        this AsyncServerStreamingCall<TResponse> value,
        Func<TRequest, IEnumerable<TResponse>> func)
    {
        return value.Returns(
            callInfo =>
            {
                var enumerator = func((TRequest)callInfo[0]).ToList().GetEnumerator();

                var responseStream = Substitute.For<IAsyncStreamReader<TResponse>>();

                responseStream
                    .Current
                    .Returns(_ => enumerator.Current);
                responseStream
                    .MoveNext(Arg.Any<CancellationToken>())
                    .Returns(_ => Task.FromResult(enumerator.MoveNext()));

                return new AsyncServerStreamingCall<TResponse>(
                    responseStream: responseStream,
                    responseHeadersAsync: Task.FromResult(new Metadata()),
                    getStatusFunc: () => Status.DefaultSuccess,
                    getTrailersFunc: () => new Metadata(),
                    disposeAction: () => { });
            });
    }
}
