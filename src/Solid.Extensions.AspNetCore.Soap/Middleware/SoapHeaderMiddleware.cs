using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Extensions.AspNetCore.Soap.Middleware
{
    public abstract class SoapHeaderMiddleware : SoapMiddleware
    {
        protected SoapHeaderMiddleware(RequestDelegate next, ILogger logger)
            : base(next, logger)
        {
        }
        protected abstract string Namespace { get; }
        protected abstract string Name { get; }
        protected abstract ValueTask<bool> HandleHeaderAsync(SoapContext context, MessageHeaderInfo info, int index);

        protected override async ValueTask InvokeAsync(SoapContext context)
        {
            var index = FindHeaderIndex(context, Name, Namespace);
            if(index >= 0)
            { 
                Logger.LogDebug($"Attempting to handle header '{Name}'.");

                var header = context.Request.Headers[index];
                var handled = false;
                try
                {
                    handled = await HandleHeaderAsync(context, header, index);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, $"Could not understand header '{Name}'.");
                }
                finally
                {
                    if (handled)
                        context.Request.Headers.UnderstoodHeaders.Add(header);
                }
            }
            await Next(context);
        }

        protected virtual int FindHeaderIndex(SoapContext context, string name, string ns)
            => context.Request.Headers.FindHeader(name, ns);
    }
}
