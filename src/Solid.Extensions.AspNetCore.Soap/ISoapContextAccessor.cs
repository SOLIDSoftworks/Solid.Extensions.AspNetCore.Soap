using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap
{
    public interface ISoapContextAccessor
    {
        SoapContext SoapContext { get; }
    }
}
