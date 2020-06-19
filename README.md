# Solid.Extensions.AspNetCore.Soap
 
Host SOAP services in AspNetCore 2.1 or 3.1.

## Simple example

```csharp

[ServiceContract]
public interface IEchoServiceContract
{
    [OperationContract]
    Task<string> EchoAsync(string value);
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
#if NETCOREAPP3_1
            .AddRouting()
#endif
            .AddSingletonSoapService<IEchoServiceContract, EchoService>()
        ;
    }

    public void Configure(IApplicationBuilder builder)
    {
#if NETCOREAPP3_1
        builder
            .UseRouting()
            .UseEndpoints(endpoints => endpoints.MapSoapService<IEchoServiceContract>("/echo"))
        ;
#else
        builder.MapSoapService<IEchoServiceContract>("/echo");
#endif
    }
}

```
## Say goodbye to server side Bindings

The above example can be called using the following Binding

```csharp
var binding = new CustomBinding(new BasicHttpBinding())
{
};
var encoding = binding.Elements.Find<TextMessageEncodingBindingElement>();
encoding.MessageVersion = MessageVersion.Default;
```

If you would host your service with https, then the server wouldn't need to change, just the client. The client side binding would be the following:

```csharp
var binding = new CustomBinding(new BasicHttpsBinding())
{
};
var encoding = binding.Elements.Find<TextMessageEncodingBindingElement>();
encoding.MessageVersion = MessageVersion.Default;
```

## Changing the message version

The message version can also be changed on the server side per mapped endpoint.

```csharp
    public void Configure(IApplicationBuilder builder)
    {
#if NETCOREAPP3_1
        builder
            .UseRouting()
            .UseEndpoints(endpoints => 
            {
                endpoints.MapSoapService<IEchoServiceContract>("/echo_legacy", MessageVersion.Soap11);
                endpoints.MapSoapService<IEchoServiceContract>("/echo", MessageVersion.Default);
            })
        ;
#else
        builder.MapSoapService<IEchoServiceContract>("/echo_legacy", MessageVersion.Soap11);
        builder.MapSoapService<IEchoServiceContract>("/echo", MessageVersion.Default);
#endif
    }
```

Then the client side binding could be:

```csharp
// BasicHttpBinding uses MessageVersion.Soap11 as it's default.
var binding = new BasicHttpBinding();
```
