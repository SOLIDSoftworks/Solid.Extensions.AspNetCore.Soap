using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;

namespace Solid.Testing.AspNetCore.Extensions.XUnit.Soap
{
    internal class SoapChannelCreationContext<TChannel> : SoapChannelCreationContext
    {
        public SoapChannelCreationContext(string path, MessageVersion messageVersion, bool reusable) 
            : base(path, messageVersion, reusable)
        {
        }

        protected override string GenerateId(string path, MessageVersion messageVersion, bool reusable)
        {
            if (!reusable) return $"{typeof(TChannel).FullName}__{messageVersion}__{path ?? "/"}__{Guid.NewGuid()}";
            return $"{typeof(TChannel).FullName}__{messageVersion}__{path ?? "/"}";
        }
    }

    public abstract class SoapChannelCreationContext
    {
        protected SoapChannelCreationContext(string path, MessageVersion messageVersion, bool reusable)
        {
            Id = GenerateId(path, messageVersion, reusable);
            Path = path;
            MessageVersion = messageVersion;
            Reusable = reusable;
        }

        public bool Reusable { get; }
        public string Path { get; }
        public string Id { get; }
        public MessageVersion MessageVersion { get;  }
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public static SoapChannelCreationContext Create<TChannel>(string path = "", MessageVersion messageVersion = null, bool reusable = true)
        {
            if (messageVersion == null)
                messageVersion = MessageVersion.Default;
            return new SoapChannelCreationContext<TChannel>(path, messageVersion, reusable);
        }

        protected abstract string GenerateId(string path, MessageVersion messageVersion, bool reusable);
    }
}
