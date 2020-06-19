# Solid.Extensions.AspNetCore.Soap
[![Build status](https://ci.appveyor.com/api/projects/status/p02vomixb4sslmt8/branch/master?svg=true)](https://ci.appveyor.com/project/gislikonrad/solid-extensions-aspnetcore-soap/branch/master) 
![AppVeyor tests](https://img.shields.io/appveyor/tests/gislikonrad/solid-extensions-aspnetcore-soap)
![GitHub](https://img.shields.io/github/license/SOLIDSoftworks/Solid.Extensions.AspNetCore.Soap) 
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Solid.Extensions.AspNetCore.Soap)
![Supported frameworks](https://img.shields.io/badge/AspNetCore-2.1%20%7C%203.1-blue)

## How to use

### Creating a contract and implementation
The contract can be created in the same way a contract would be created in legacy WCF using .Net Framework. The implementation is simpler because there is no need for the ServiceBehavior attribute.

```csharp

[ServiceContract]
public interface IEchoServiceContract
{
    [OperationContract]
    Task<string> EchoAsync(string value);
}

public class EchoService : IEchoServiceContract
{
    public Task<string> EchoAsync(string value) => Task.FromResult(value);
}

```

### Configuring the service

Configuring the service is quite simple, as only vanilla AspNetCore is required. Mvc is not needed.

#### Configuring the service using AspNetCore 3.1 and endpoint routing

```csharp

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddRouting()
            .AddSingletonSoapService<IEchoServiceContract, EchoService>()
        ;
    }

    public void Configure(IApplicationBuilder builder)
    {
        builder
            .UseRouting()
            .UseEndpoints(endpoints => endpoints.MapSoapService<IEchoServiceContract>("/echo", MessageVersion.Soap11))
        ;
    }
}

```

#### Configuring the service using AspNetCore 2.1

```csharp

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingletonSoapService<IEchoServiceContract, EchoService>();
    }

    public void Configure(IApplicationBuilder builder)
    {
        builder.MapSoapService<IEchoServiceContract>("/echo", MessageVersion.Soap11);
    }
}

```

#### Say goodbye to server side Bindings

As you can see above, we are not using System.ServiceModel.Channels.Binding on the server side. We felt that it was unnecessary. If you're going to host the service with https, why would you need to program that in? Just host with https!

## Creating client channels for services hosted in AspNetCore

Since we don't have wsdl generation yet, channels should be created manually using either code or config. Thankfully, there is nothing custom about this. In c#, this is just vanilla WCF.

### Basic bindings

```csharp
// The binding could also be BasicHttpsBinding
var binding = new BasicHttpBinding();
var baseUrl = new Uri("http://localhost:5000");
var url = new Uri(baseUrl, "echo");
var endpointAddress = new EndpointAddress(url);
var factory = new ChannelFactory<IEchoServiceContract>(binding, endpointAddress);
var channel = factory.CreateChannel();
```

### Using a different MessageVersion

The BasicHttpBinding and BasicHttpsBinding use Soap11 as their default message version, which is why the example above uses MessageVersion.Soap11. However, if the service and/or endpoint is defined using a different message version in the Configure method of the Startup class, this can be changed manually in the client binding aswell.

```csharp
var binding = new CustomBinding(new BasicHttpsBinding())
{
};
var encoding = binding.Elements.Find<TextMessageEncodingBindingElement>();
encoding.MessageVersion = MessageVersion.Soap12WSAddressing10;
var baseUrl = new Uri("http://localhost:5000");
var url = new Uri(baseUrl, "echo");
var endpointAddress = new EndpointAddress(url);
var factory = new ChannelFactory<IEchoServiceContract>(binding, endpointAddress);
var channel = factory.CreateChannel();
```