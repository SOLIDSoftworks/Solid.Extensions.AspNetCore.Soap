using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap
{
    /// <summary>
    /// Constants for SOAP serialization and deserialization.
    /// </summary>
    public static class SoapConstants
    {
        /// <summary>
        /// SOAP 1.1 namespace
        /// </summary>
        public const string Soap11EnvelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";

        /// <summary>
        /// SOAP 1.2 namespace
        /// </summary>
        public const string Soap12EnvelopeNamespace = "http://www.w3.org/2003/05/soap-envelope";
    }

    /// <summary>
    /// Constants for XML serialization and deserialization.
    /// </summary>
    public static class XmlConstants
    { 
        /// <summary>
        /// XML schema namespace
        /// </summary>
        public const string XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

        /// <summary>
        /// XML namespace
        /// </summary>
        public const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";
    }
}
