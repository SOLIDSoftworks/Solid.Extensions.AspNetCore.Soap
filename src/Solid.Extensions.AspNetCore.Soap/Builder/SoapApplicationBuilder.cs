using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap.Builder
{
    internal class SoapApplicationBuilder : ISoapApplicationBuilder, IApplicationBuilder
    {
        private IApplicationBuilder _inner;

        public SoapApplicationBuilder(IApplicationBuilder inner) => _inner = inner;
        public IServiceProvider ApplicationServices { get => _inner.ApplicationServices; set => _inner.ApplicationServices = value; }

        public IFeatureCollection ServerFeatures => _inner.ServerFeatures;

        public IDictionary<string, object> Properties => _inner.Properties;

        public RequestDelegate Build() => _inner.Build();

        public IApplicationBuilder New() => _inner.New();

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware) => _inner.Use(middleware);
    }
}
