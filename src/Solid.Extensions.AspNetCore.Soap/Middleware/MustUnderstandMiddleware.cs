﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Solid.Extensions.AspNetCore.Soap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Extensions.AspNetCore.Soap.Middleware
{
    internal class MustUnderstandMiddleware : SoapMiddleware
    {
        public MustUnderstandMiddleware(RequestDelegate next, ILogger<MustUnderstandMiddleware> logger) 
            : base(next, logger)
        {
        }

        protected override async ValueTask InvokeAsync(SoapContext context)
        {
            if(context.Options.ValidateMustUnderstand)
            {
                if (!context.Request.Headers.HaveMandatoryHeadersBeenUnderstood())
                {
                    var reason = context.Options.MessageVersion.CreateFaultReason(reason: "An immediate child element of the Header element, with the mustUnderstand attribute set to \"1\", was not understood");
                    var code = context.Options.MessageVersion.CreateFaultCode(code: "MustUnderstand");

                    var headers = context
                        .Request
                        .Headers
                        .Where(h => h.MustUnderstand)
                        .Except(context.Request.Headers.UnderstoodHeaders)
                    ;

                    Logger.LogDebug(CreateMessage(headers));
                    throw new FaultException(reason, code, context.Request.Headers.Action);
                }
                Logger.LogInformation("All soap headers understood.");
            }
            await Next(context);
        }

        private string CreateMessage(IEnumerable<MessageHeaderInfo> headers)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Not all headers understood.");
            builder.AppendLine("Headers marked mustUnderstand=\"1\" and not understood:");
            foreach (var header in headers)
                builder.AppendLine($"  localName: {header.Name} - namespace: {header.Namespace}");
            return builder.ToString();
        }
    }
}
