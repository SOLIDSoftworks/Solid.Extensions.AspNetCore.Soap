using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal static class MessageBufferExtensions
    {
        public static string ReadAll(this MessageBuffer buffer, bool indent = true)
        {
            var message = buffer.CreateMessage();
            using(var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = false, Indent = indent, Encoding = Encoding.UTF8 }))
                    message.WriteMessage(writer);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }
    }
}
