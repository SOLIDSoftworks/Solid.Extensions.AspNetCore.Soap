using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap
{
    public class SoapServiceBuilder
    {
        internal SoapServiceBuilder(Type contract, IServiceCollection services)
        {
            Contract = contract;
            Services = services;            
        }

        public Type Contract { get; }
        public IServiceCollection Services { get; }

        public SoapServiceBuilder Configure(Action<SoapServiceOptions> configureOptions)
        {
            Services.Configure(Contract.FullName, configureOptions);
            return this;
        }
    }
}
