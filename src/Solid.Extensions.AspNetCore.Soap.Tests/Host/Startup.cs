using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap.Tests.Host
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services.AddScopedSoapService<IEchoServiceContract, EchoService>(_ => { });
            services.AddScopedSoapService<IFaultServiceContract, FaultService>(_ => { });
            services.AddScopedSoapService<IDetailedFaultServiceContract, FaultService>(builder => builder.Configure(options => options.IncludeExceptionDetailInFaults = true));
        }

        public void Configure(IApplicationBuilder builder)
        {
            builder
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapSoapService<IEchoServiceContract>("/echo");
                    //endpoints.MapSoapService<IEchoServiceContract>("/echo1", MessageVersion.None);
                    endpoints.MapSoapService<IEchoServiceContract>("/echo2", MessageVersion.Soap11);
                    endpoints.MapSoapService<IEchoServiceContract>("/echo3", MessageVersion.Soap11WSAddressingAugust2004);
                    endpoints.MapSoapService<IEchoServiceContract>("/echo4", MessageVersion.Soap12WSAddressing10);
                    endpoints.MapSoapService<IEchoServiceContract>("/echo5", MessageVersion.Soap12WSAddressingAugust2004);
                    endpoints.MapSoapService<IFaultServiceContract>("/faults");
                    endpoints.MapSoapService<IDetailedFaultServiceContract>("/detailedfaults");
                })
            ;
        }
    }
}
