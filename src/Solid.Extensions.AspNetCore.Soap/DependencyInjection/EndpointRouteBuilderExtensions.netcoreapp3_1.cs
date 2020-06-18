#if NETCOREAPP3_1
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Solid.Extensions.AspNetCore.Soap.Builder;
using Solid.Extensions.AspNetCore.Soap.Middleware;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Solid_Extensions_AspNetCore_Soap_EndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapSoapService<TService>(this IEndpointRouteBuilder endpoints, PathString path)
            => endpoints.MapSoapService<TService>(path, _ => {});
        public static IEndpointRouteBuilder MapSoapService<TService>(this IEndpointRouteBuilder endpoints, PathString path, Action<ISoapApplicationBuilder> configure)
        {
            var builder = endpoints.CreateApplicationBuilder();
            builder.UseSoapService<TService>(configure);
            var requestDelegate = builder.Build();

            endpoints.Map(RoutePatternFactory.Parse(path), requestDelegate);
            return endpoints;
        }
    }
}
#endif