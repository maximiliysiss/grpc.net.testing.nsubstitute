using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
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
                var requests = new List<TRequest>();

                var taskCompletionSource = new TaskCompletionSource<TResponse>();

                var reader = Substitute.For<IClientStreamWriter<TRequest>>();

                reader
                    .WriteAsync(Arg.Any<TRequest>())
                    .Returns(Task.CompletedTask)
                    .AndDoes(c => requests.Add((TRequest)c[0]));
                reader
                    .CompleteAsync()
                    .Returns(Task.CompletedTask)
                    .AndDoes(_ => taskCompletionSource.TrySetResult(func(requests)));

                return new AsyncClientStreamingCall<TRequest, TResponse>(
                    requestStream: reader,
                    responseAsync: taskCompletionSource.Task,
                    responseHeadersAsync: Task.FromResult(new Metadata()),
                    getStatusFunc: () => Status.DefaultSuccess,
                    getTrailersFunc: () => new Metadata(),
                    disposeAction: () => { });
            });
}
