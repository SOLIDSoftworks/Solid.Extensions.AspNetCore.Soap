using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Solid.Extensions.AspNetCore.Soap.Logging;
using Solid.Extensions.AspNetCore.Soap.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Extensions.AspNetCore.Soap.Middleware
{
    public delegate Task SoapRequestDelegate(SoapContext context);
    public abstract class SoapMiddleware
    {
        private RequestDelegate _next;

        public SoapMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            Logger = logger;
            Next = soap => _next(soap.HttpContext);
        }

        protected SoapRequestDelegate Next { get; }
        protected ILogger Logger { get; }

        protected abstract ValueTask InvokeAsync(SoapContext context);

        public Task InvokeAsync(HttpContext context)
        {
            var soap = context.GetSoapContext();
            if (soap == null) _next(context); // TODO: error?

            if(soap?.Response != null)
            {
                LoggerMessages.LogShortCircuitingPipeline(Logger);
                return Task.CompletedTask;
            }
            LoggerMessages.LogInvokingMiddleware(Logger, this.GetType());
            return InvokeAsync(soap).AsTask();
        }
    }
}
