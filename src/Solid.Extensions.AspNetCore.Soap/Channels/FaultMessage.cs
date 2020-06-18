using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace Solid.Extensions.AspNetCore.Soap.Channels
{
    public abstract class FaultMessage : Message
    {
        protected MessageFault Fault { get; }
        protected string Action { get; }

        public static Message CreateFaultMessage(MessageVersion version, MessageFault fault, string action)
        {           
            if (version.Envelope == EnvelopeVersion.Soap12)
                return new Soap12FaultMessage(version, fault, action);
            if (version.Envelope == EnvelopeVersion.Soap11)
                return new Soap11FaultMessage(version, fault, action);

            throw new Exception("Message version has no fault schema");
        }

        private FaultMessage(MessageVersion version, MessageFault fault, string action)
        {
            Fault = fault;
            Action = action;

            Version = version;
            Headers = new MessageHeaders(version);
        }

        public override MessageHeaders Headers { get; }

        public override MessageProperties Properties { get; } = new MessageProperties();

        public override MessageVersion Version { get; }

        public override bool IsFault => true;

        class Soap11FaultMessage : FaultMessage
        {
            public Soap11FaultMessage(MessageVersion version, MessageFault fault, string action)
                : base(version, fault, action)
            {
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                writer.WriteStartElement("Fault", Constants.Soap11EnvelopeNamespace);

                WriteFaultCodeElement(writer, Fault.Code);
                //writer.WriteElementString("faultcode", GetFaultCodeString(Fault.Code));
                writer.WriteElementString("faultstring", Fault.Reason.GetMatchingTranslation().Text);
                writer.WriteElementString("faultactor", Fault.Actor);

                if (Fault.HasDetail)
                {
                    using (var reader = Fault.GetReaderAtDetailContents())
                    {
                        writer.WriteStartElement("detail");
                        writer.WriteRaw(reader.ReadOuterXml());
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
            }

            private void WriteFaultCodeElement(XmlDictionaryWriter writer, FaultCode code)
            {
                var prefix = writer.LookupPrefix(Constants.Soap11EnvelopeNamespace);
                if (!string.IsNullOrEmpty(code.Namespace) && code.Namespace != Constants.Soap11EnvelopeNamespace)
                    prefix = writer.LookupPrefix(code.Namespace) ?? "custom";

                var name = code.Name;
                writer.WriteStartElement("faultcode");

                if (!code.IsPredefinedFault)
                {
                    if (prefix == "custom")
                        writer.WriteXmlnsAttribute(prefix, code.Namespace);
                }
                else
                {
                    if (code.IsReceiverFault) name = "Server";
                    if (code.IsSenderFault) name = "Client";
                }

                writer.WriteString($"{prefix}:{name}");
                writer.WriteEndElement();
            }
        }
        class Soap12FaultMessage : FaultMessage
        {
            public Soap12FaultMessage(MessageVersion version, MessageFault fault, string action) : base(version, fault, action)
            {
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                writer.WriteStartElement("Fault", Constants.Soap12EnvelopeNamespace);

                WriteFaultCodeElement(writer, Fault.Code, "Code");
                //writer.WriteElementString("Value", Constants.Soap12EnvelopeNamespace, GetFaultCodeString(Fault.Code));

                var reason = Fault.Reason.GetMatchingTranslation();
                writer.WriteStartElement("Reason", Constants.Soap12EnvelopeNamespace);
                writer.WriteStartElement("Text", Constants.Soap12EnvelopeNamespace);
                writer.WriteAttributeString("xml", "lang", Constants.XmlNamespace, reason.XmlLang);
                writer.WriteString(reason.Text);
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteElementString("Node", Constants.Soap12EnvelopeNamespace, Fault.Node);

                if (Fault.HasDetail)
                {
                    using (var reader = Fault.GetReaderAtDetailContents())
                    using(var inner = reader.ReadSubtree())
                    {
                        writer.WriteStartElement("Detail", Constants.Soap12EnvelopeNamespace);
                        while(inner.Read())
                        {
                            if (inner.NodeType == XmlNodeType.Element)
                            {
                                if (string.IsNullOrEmpty(inner.NamespaceURI))
                                    writer.WriteStartElement(inner.LocalName);
                                else
                                    writer.WriteStartElement(inner.LocalName, inner.NamespaceURI);

                                writer.WriteAttributes(inner, false);
                                continue;
                            }

                            if (inner.NodeType == XmlNodeType.Text)
                            {
                                writer.WriteValue(inner.Value);
                                continue;
                            }

                            if (inner.NodeType == XmlNodeType.EndElement)
                            {
                                writer.WriteEndElement();
                                continue;
                            }
                        }
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
            }

            private void WriteFaultCodeElement(XmlDictionaryWriter writer, FaultCode code, string localName)
            {
                var prefix = writer.LookupPrefix(Constants.Soap12EnvelopeNamespace);
                if (!string.IsNullOrEmpty(code.Namespace) && code.Namespace != Constants.Soap12EnvelopeNamespace)
                    prefix = writer.LookupPrefix(code.Namespace) ?? "custom";

                var name = code.Name;
                writer.WriteStartElement(localName, Constants.Soap12EnvelopeNamespace);
                writer.WriteStartElement("Value", Constants.Soap12EnvelopeNamespace);

                if (!code.IsPredefinedFault)
                {
                    if (prefix == "custom")
                        writer.WriteXmlnsAttribute(prefix, code.Namespace);
                }
                else
                {
                    if (code.IsReceiverFault) name = "Receiver";
                    if (code.IsSenderFault) name = "Sender";
                }

                writer.WriteString($"{prefix}:{name}");
                writer.WriteEndElement();
                if (code.SubCode != null)
                    WriteFaultCodeElement(writer, code.SubCode, "Subcode");
                writer.WriteEndElement();
            }
        }
    }
}