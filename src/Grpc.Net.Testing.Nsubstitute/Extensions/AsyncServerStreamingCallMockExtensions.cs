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
                var responses = new Queue<TResponse>(func((TRequest)callInfo[0]));
                var isThereNext = new Queue<bool>(Enumerable.Range(1, responses.Count + 1).Select(i => i <= responses.Count));

                var responseStream = Substitute.For<IAsyncStreamReader<TResponse>>();
                responseStream.Current.Returns(_ => responses.Dequeue());
                responseStream.MoveNext(Arg.Any<CancellationToken>()).Returns(_ => isThereNext.Dequeue());

                var fakeCall = new AsyncServerStreamingCall<TResponse>(
                    responseStream,
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { });

                return fakeCall;
            });
    }
}
