using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap.Builder
{
    /// <summary>
    /// An interface that enables extending a SOAP endpoint.
    /// <para>
    /// This is nothing more that an <see cref="IApplicationBuilder" />. 
    /// It's a seperate interface so that specific SOAP middleware extension methods can be created.
    /// </para>
    /// </summary>
    public interface ISoapApplicationBuilder : IApplicationBuilder
    {
    }
}
