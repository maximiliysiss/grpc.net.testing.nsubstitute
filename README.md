# grpc.net.testing.nsubstitute

Library to mocking gRPC client. Instead of `Grpc.Core.Testing` using extensions for `NSubstitute`

Based on libraries:

* [NSubstitute](https://github.com/nsubstitute/NSubstitute)

## Install

### Nuget:

`Install-Package Net.Testing.NSubstitute.Grpc`

## Using example

### Simple sync calling:

#### 1. Call with exists response
```c#
// Create client
var grpcMock = Substitute.For<TestService.TestServiceClient>();

// Add mock
grpcMock
    .Simple(Arg.Any<TestRequest>(), null, null, default)
    .Returns(expectedResponse);

// Call
var response = grpcMock.Simple(new TestRequest());
```


### Simple async calling:

#### 1. Call with exists response

```c#
// Create client
var testRequest = new TestRequest();
var grpcClient = Substitute.For<TestService.TestServiceClient>();

// Add mock
grpcClient
    .SimpleAsync(Arg.Any<TestRequest>(), Arg.Any<Metadata>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
    .Returns(new TestResponse());

// Call
var response = await grpcClient.SimpleAsync(testRequest);
```

#### 2. Call with response creating on call
```c#
// Create client
var testRequest = new TestRequest();
var grpcClient = Substitute.For<TestService.TestServiceClient>();

// Add mock
grpcClient
    .SimpleAsync(Arg.Any<TestRequest>(), Arg.Any<Metadata>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
    .Returns(() => new TestResponse());

// Call
var response = await grpcClient.SimpleAsync(testRequest);
```

#### 3. Call with response creating on call and based on request
```c#
// Create client
var testRequest = new TestRequest();
var grpcClient = Substitute.For<TestService.TestServiceClient>();

// Add mock
grpcClient
    .SimpleAsync(Arg.Any<TestRequest>(), Arg.Any<Metadata>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
    .Returns<TestRequest, TestResponse>(r => new TestResponse{ Val = r.Val });

// Call
var response = await grpcClient.SimpleAsync(testRequest);
```

### Simple client stream calling:

#### 1. Call with exists response

```c#
// Create client
var grpcMock = Substitute.For<TestService.TestServiceClient>();

// Add mock
grpcMock
    .SimpleClientStream(Arg.Any<Metadata>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
    .Returns(expectedResponses);

// Call
var call = grpcMock.SimpleClientStream();

await call.RequestStream.WriteAllAsync(requests, complete: true);
var message = await call;
```

#### 2. Call with response creating on call

```c#
// Create client
var grpcMock = Substitute.For<TestService.TestServiceClient>();

// Add mock
grpcMock
    .SimpleClientStream(Arg.Any<Metadata>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
    .Returns(() => expectedResponses);

// Call
var call = grpcMock.SimpleClientStream();

await call.RequestStream.WriteAllAsync(requests, complete: true);
var message = await call;
```

#### 3. Call with response creating on call and based on request

```c#
// Create client
var grpcMock = Substitute.For<TestService.TestServiceClient>();

// Add mock
grpcMock
    .SimpleClientStream(Arg.Any<Metadata>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
    .Returns(rs => new TestResponse { Val = rs.Sum(r => r.Val) });

// Call
var call = grpcMock.SimpleClientStream();

await call.RequestStream.WriteAllAsync(requests, complete: true);
var message = await call;
```

### Simple server stream calling:

#### 1. Call with exists response

```c#
// Create client
var grpcMock = Substitute.For<TestService.TestServiceClient>();

// Add mock
grpcMock
    .SimpleServerStream(Arg.Any<TestRequest>(), null, null, default)
    .Returns(expectedResponses);

// Call
var call = grpcMock.SimpleServerStream(new TestRequest());
var messages = await call.ResponseStream.ReadAllAsync().ToArrayAsync();
```

#### 2. Call with response creating on call

```c#
// Create client
var grpcMock = Substitute.For<TestService.TestServiceClient>();

// Add mock
grpcMock
    .SimpleServerStream(Arg.Any<TestRequest>(), null, null, default)
    .Returns(() => expectedResponses);

// Call
var call = grpcMock.SimpleServerStream(new TestRequest());
var messages = await call.ResponseStream.ReadAllAsync().ToArrayAsync();
```

#### 3. Call with response creating on call and based on request

```c#
// Create client
var grpcMock = Substitute.For<TestService.TestServiceClient>();

// Add mock
grpcMock
    .SimpleServerStream(Arg.Any<TestRequest>(), null, null, default)
    .Returns<TestRequest, TestResponse>(r => Enumerable.Repeat(new TestResponse { Val = r.Val }, 2));

// Call
var call = grpcMock.SimpleServerStream(new TestRequest());
var messages = await call.ResponseStream.ReadAllAsync().ToArrayAsync();
```

### Simple client server streams calling:

#### 1. Call with exists response

```c#
// Creation client
var grpcMock = Substitute.For<TestService.TestServiceClient>();

// Add mock
grpcMock
    .SimpleClientServerStream(null, null, default)
    .Returns(expectedResponses);

// Call
var call = grpcMock.SimpleClientServerStream();

await call.RequestStream.WriteAllAsync(requests, complete: true);
var messages = await call.ResponseStream.ReadAllAsync().ToArrayAsync();
```

#### 2. Call with response creating on call

```c#
// Creation client
var grpcMock = Substitute.For<TestService.TestServiceClient>();

// Add mock
grpcMock
    .SimpleClientServerStream(null, null, default)
    .Returns(() => expectedResponses);

// Call
var call = grpcMock.SimpleClientServerStream();

await call.RequestStream.WriteAllAsync(requests, complete: true);
var messages = await call.ResponseStream.ReadAllAsync().ToArrayAsync();
```

#### 3. Call with response creating on call and based on request

```c#
// Creation client
var grpcMock = Substitute.For<TestService.TestServiceClient>();

// Add mock
grpcMock
    .SimpleClientServerStream(null, null, default)
    .Returns(rs => new[] { new TestResponse { Val = rs.Sum(r => r.Val) } });

// Call
var call = grpcMock.SimpleClientServerStream();

await call.RequestStream.WriteAllAsync(requests, complete: true);
var messages = await call.ResponseStream.ReadAllAsync().ToArrayAsync();
```
