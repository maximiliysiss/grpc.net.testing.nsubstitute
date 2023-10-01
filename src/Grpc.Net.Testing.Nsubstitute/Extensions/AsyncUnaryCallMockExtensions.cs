using System;
using System.Threading.Tasks;
using Grpc.Core;
using NSubstitute;
using NSubstitute.Core;

namespace Grpc.Net.Testing.Nsubstitute.Extensions;

public static class AsyncUnaryCallMockExtensions
{
    public static ConfiguredCall Returns<TResponse>(this AsyncUnaryCall<TResponse> value, TResponse returnThis)
        => Returns(value, () => returnThis);

    public static ConfiguredCall Returns<TResponse>(this AsyncUnaryCall<TResponse> value, Func<TResponse> func)
        => Returns<object, TResponse>(value, _ => func());

    public static ConfiguredCall Returns<TRequest, TResponse>(this AsyncUnaryCall<TResponse> value, Func<TRequest, TResponse> func)
        => value.Returns(
            callInfo => new AsyncUnaryCall<TResponse>(
                Task.FromResult(func((TRequest)callInfo[0])),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }));
}
