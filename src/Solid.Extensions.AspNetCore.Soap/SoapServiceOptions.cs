using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap
{
    public class SoapServiceOptions
    {
        public string Name { get; internal set; }
        public string Namespace { get; internal set; }
        public MessageVersion MessageVersion { get; set; } = MessageVersion.Default;
        public int MaxSizeOfHeaders { get; set; } = int.MaxValue;
        public bool ValidateMustUnderstand { get; set; } = true;
        public bool IncludeExceptionDetailInFaults { get; set; } = false;
        //public List<MessageEncoder> Encoders { get; set; }
        //public TimeSpan TransactionTimeout { get; set; } = TimeSpan.Zero;

        // TODO: UnkownMessageREceived handler
    }
}
