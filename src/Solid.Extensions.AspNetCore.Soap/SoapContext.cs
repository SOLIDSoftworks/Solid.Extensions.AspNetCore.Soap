using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;

namespace Solid.Extensions.AspNetCore.Soap
{
    internal class SoapContext<TContract> : SoapContext
    {
        public SoapContext(HttpContext http, Message request, SoapServiceOptions options) 
            : base(http, request, options)
        {
        }

        public override Type Contract => typeof(TContract);
    }
    public abstract class SoapContext
    {
        public SoapContext(HttpContext http, Message request, SoapServiceOptions options)
        {
            HttpContext = http;
            Request = request;
            Options = options;
        }

        public HttpContext HttpContext { get; }
        public abstract Type Contract { get; }
        public string ContentType => HttpContext.Request.ContentType;
        public SoapServiceOptions Options { get; }
        public IServiceProvider RequestServices => HttpContext.RequestServices;
        public Message Request { get; set; }
        public Message Response { get; set; }
        public ClaimsPrincipal User { get => HttpContext.User; set => HttpContext.User = value; }
        public CancellationToken CancellationToken => HttpContext.RequestAborted;
    }
}
