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
        services.AddSingletonSoapService<IEchoServiceContract, EchoService>();
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

The message version can also be changed on the server side using the new options pattern.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingletonSoapService<IEchoServiceContract, EchoService>(options => options.MessageVersion = MessageVersion.Soap11);
}
```