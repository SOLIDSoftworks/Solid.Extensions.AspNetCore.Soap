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
    /// <summary>
    /// Extensions for adding endpoints for SOAP services.
    /// </summary>
    public static class Solid_Extensions_AspNetCore_Soap_EndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Maps a SOAP endpoint to a <paramref name="path" />.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder" /> instance.</param>
        /// <param name="path">The path to map <typeparamref name="TService" /> to.</param>
        /// <typeparam name="TService">The SOAP service contract.</typeparam>
        /// <returns>The <see cref="IEndpointRouteBuilder" /> instance so that additional calls can be chained.</returns>
        public static IEndpointRouteBuilder MapSoapService<TService>(this IEndpointRouteBuilder endpoints, PathString path)
            => endpoints.MapSoapService<TService>(path, _ => {});

        /// <summary>
        /// Maps a SOAP endpoint to a <paramref name="path" />.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder" /> instance.</param>
        /// <param name="path">The path to map <typeparamref name="TService" /> to.</param>
        /// <param name="configure">A delegate for adding middleware into the SOAP request pipeline.</param>
        /// <typeparam name="TService">The SOAP service contract.</typeparam>
        /// <returns>The <see cref="IEndpointRouteBuilder" /> instance so that additional calls can be chained.</returns>
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