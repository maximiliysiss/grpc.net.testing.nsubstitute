using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
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
                var requests = new List<TRequest>();
                var responses = new List<TResponse>();

                var writer = Substitute.For<IClientStreamWriter<TRequest>>();
                var reader = Substitute.For<IAsyncStreamReader<TResponse>>();

                writer
                    .WriteAsync(Arg.Any<TRequest>())
                    .Returns(Task.CompletedTask)
                    .AndDoes(c => requests.Add((TRequest)c[0]));
                writer
                    .CompleteAsync()
                    .Returns(Task.CompletedTask)
                    .AndDoes(_ => responses.AddRange(func(requests)));

                var index = -1;

                reader
                    .Current
                    .Returns(_ => responses[index]);
                reader
                    .MoveNext(Arg.Any<CancellationToken>())
                    .Returns(c => Task.FromResult(++index < responses.Count));

                return new AsyncDuplexStreamingCall<TRequest, TResponse>(
                    requestStream: writer,
                    responseStream: reader,
                    responseHeadersAsync: Task.FromResult(new Metadata()),
                    getStatusFunc: () => Status.DefaultSuccess,
                    getTrailersFunc: () => new Metadata(),
                    disposeAction: () => { });
            });
}
