using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Channels
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Solid_Extensions_AspNetCore_Soap_MessageBufferExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Reads the whole message contained in a <see cref="MessageBuffer"/> to <seealso cref="string"/>.
        /// </summary>
        /// <param name="buffer">The <see cref="MessageBuffer"/> to read from.</param>
        /// <param name="indent">A flag to set whether the output XML will be indented or not.</param>
        /// <returns>An XML <see cref="string"/>.</returns>
        public static string ReadAll(this MessageBuffer buffer, bool indent = true)
        {
            var message = buffer.CreateMessage();
            using (var reader = buffer.GetReader(indent: indent))
            {
                reader.MoveToContent();
                return reader.ReadOuterXml();
            }
        }

        /// <summary>
        /// Gets an <see cref="XmlReader"/> from a <seealso cref="MessageBuffer"/> positioned at index 0.
        /// </summary>
        /// <param name="buffer">The <see cref="MessageBuffer"/> to created the <seealso cref="XmlReader"/> for.</param>
        /// <param name="indent">A flag to set whether the XML will be indented or not.</param>
        /// <returns>An <see cref="XmlReader"/>.</returns>
        public static XmlReader GetReader(this MessageBuffer buffer, bool indent = true)
        {
            var message = buffer.CreateMessage();
            var stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = false, Indent = indent, Encoding = new UTF8Encoding(false) }))
                message.WriteMessage(writer);
            stream.Position = 0;
            return XmlReader.Create(stream);
        }
    }
}
