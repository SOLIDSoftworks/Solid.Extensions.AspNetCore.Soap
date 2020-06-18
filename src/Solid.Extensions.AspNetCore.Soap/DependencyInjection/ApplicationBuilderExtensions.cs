using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Solid.Extensions.AspNetCore.Soap.Builder;
using Solid.Extensions.AspNetCore.Soap.Middleware;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder MapSoapService<TService>(this IApplicationBuilder builder, PathString path, Action<ISoapApplicationBuilder> configure)
        {
            builder.Map(path, b => b.UseSoapService<TService>(configure));
            return builder;
        }

        internal static IApplicationBuilder UseSoapService<TService>(this IApplicationBuilder builder, Action<ISoapApplicationBuilder> configure)
        {
            //builder.Use(async (context, next) =>
            //{
            //    var requestServices = context.RequestServices;
            //    using (var scope = requestServices.CreateScope())
            //    {
            //        context.RequestServices = scope.ServiceProvider;
            //        await next();
            //        context.RequestServices = requestServices;
            //    }
            //});
            builder.UseMiddleware<SoapRequestMiddleware<TService>>();
            var soap = new SoapApplicationBuilder(builder);
            configure(soap);
            builder.UseMiddleware<MustUnderstandMiddleware>();
            builder.UseMiddleware<SoapServiceInvokerMiddleware>();
            return builder;
        }
    }
}
