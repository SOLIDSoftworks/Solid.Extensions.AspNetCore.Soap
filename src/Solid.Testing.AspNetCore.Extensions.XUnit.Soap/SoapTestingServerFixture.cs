using Solid.Testing.AspNetCore.Extensions.XUnit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Testing.AspNetCore.Extensions.XUnit.Soap
{
    public class SoapTestingServerFixture<TStartup> : TestingServerFixture<TStartup>
    {
        private ConcurrentDictionary<string, ICommunicationObject> _channels = new ConcurrentDictionary<string, ICommunicationObject>();

        protected virtual Binding CreateBinding<TContract>(string name, MessageVersion version)
        {
            var binding = new CustomBinding(new BasicHttpBinding())
            {
            };
            var encoding = binding.Elements.Find<TextMessageEncodingBindingElement>();
            encoding.MessageVersion = version;
            return binding.WithSolidHttpTransport(TestingServer);
        }

        protected virtual EndpointAddress CreateEndpointAddress<TChannel>(string name, Uri url)
            => new EndpointAddress(url);

        protected virtual ChannelFactory<TChannel> CreateChannelFactory<TChannel>(string name, Binding binding, EndpointAddress endpointAddress)
            => new ChannelFactory<TChannel>(binding, endpointAddress);
        
        protected virtual ICommunicationObject CreateChannel<TChannel>(ChannelFactory<TChannel> factory)
            => factory.CreateChannel() as ICommunicationObject;

        public TChannel CreateChannel<TChannel>(MessageVersion version = null, string name = "", string path = null, bool reusable = true)
            where TChannel : class
        {
            if(version == null)
                version = MessageVersion.Default;
            var key = GenerateKey<TChannel>(version, name, path, !reusable);
            return _channels.GetOrAdd(key, k =>
            {
                var binding = CreateBinding<TChannel>(k, version);
                var url = TestingServer.BaseAddress;
                if (path != null)
                    url = new Uri(url, path);

                var endpointAddress = CreateEndpointAddress<TChannel>(k, url);
                var factory = CreateChannelFactory<TChannel>(k, binding, endpointAddress);
                var channel = CreateChannel(factory);
                channel.Faulted += (sender, args) => _channels.TryRemove(k, out _);
                channel.Closing += (sender, args) => _channels.TryRemove(k, out _);
                return channel;
            }) as TChannel;
        }

        protected virtual string GenerateKey<TChannel>(MessageVersion version, string name, string path, bool unique)
        {
            if (unique) return $"{typeof(TChannel).FullName}__{version}__{name}__{path ?? "/"}__{Guid.NewGuid()}";
            return $"{typeof(TChannel).FullName}__{version}__{name}__{path ?? "/"}";
        }

        protected override void Disposing()
        {
            var channels = _channels.Values.OfType<ICommunicationObject>().ToArray();
            foreach (var channel in channels)
                channel.Close();
        }
    }
}
