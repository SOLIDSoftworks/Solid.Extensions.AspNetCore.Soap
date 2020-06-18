﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Solid.Extensions.AspNetCore.Soap.Builder;
using Solid.Extensions.AspNetCore.Soap.Middleware;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for adding SOAP service middleware.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Maps a SOAP service to a <paramref name="path" />.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder" /> instance.</param>
        /// <param name="path">The path to map <typeparamref name="TService" /> to.</param>
        /// <typeparam name="TService">The SOAP service contract.</typeparam>
        /// <returns>The <see cref="IApplicationBuilder" /> instance so that additional calls can be chained.</returns>
        public static IApplicationBuilder MapSoapService<TService>(this IApplicationBuilder builder, PathString path)
            => builder.MapSoapService<TService>(path, _ => {});

        /// <summary>
        /// Maps a SOAP service to a <paramref name="path" />.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder" /> instance.</param>
        /// <param name="path">The path to map <typeparamref name="TService" /> to.</param>
        /// <param name="configure">A delegate for adding middleware into the SOAP request pipeline.</param>
        /// <typeparam name="TService">The SOAP service contract.</typeparam>
        /// <returns>The <see cref="IApplicationBuilder" /> instance so that additional calls can be chained.</returns>
        public static IApplicationBuilder MapSoapService<TService>(this IApplicationBuilder builder, PathString path, Action<ISoapApplicationBuilder> configure)
        {
            builder.Map(path, b => b.UseSoapService<TService>(configure));
            return builder;
        }

        internal static IApplicationBuilder UseSoapService<TService>(this IApplicationBuilder builder, Action<ISoapApplicationBuilder> configure)
        {
            builder.UseMiddleware<SoapRequestMiddleware<TService>>();
            var soap = new SoapApplicationBuilder(builder);
            configure(soap);
            builder.UseMiddleware<MustUnderstandMiddleware>();
            builder.UseMiddleware<SoapServiceInvokerMiddleware>();
            return builder;
        }
    }
}
